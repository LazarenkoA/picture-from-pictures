using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.Remoting.Messaging;
using System.Threading;


namespace WindowsFormsApplication6 {
  public partial class Load:Form {

    private delegate void DLoadAndSaveImag(string[] URLs);

    public Load() {
      InitializeComponent();
      progressBar1.Maximum = (int)numericUpDown1.Value;
      Text = "Загрузка:";
    }

    private void button1_Click(object sender, EventArgs e) {
      if(string.IsNullOrEmpty(textBox1.Text)) {
        errorProvider1.SetError(textBox1, "Заполните строку поиска");
        return;
      }

      if(string.IsNullOrEmpty(toolStripStatusLabel1.Text)) {
        errorProvider1.SetError(button2, "Выбирете папку для сохранения");
        return;
      }

      if(button1.Text == "Прервать") {
        backgroundWorker1.CancelAsync();
        button1.Text = "Прерываем потоки";
        return;
      } else if(button1.Text == "Грузить") {
        button1.Text = "Прервать";
        button2.Enabled = false;
      }

      if(!backgroundWorker1.IsBusy)
        backgroundWorker1.RunWorkerAsync();
      else {
        MessageBox.Show("Дождитесь окончание работы потоков", "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        return;
      }
      //var load = new LoadImage(_LoadImage);
      //AsyncCallback CallBack = new AsyncCallback(Result => { this.Invoke(new Action(() => { button1.Text = "Грузить"; numericUpDown1.Value = 0; })); });
      //IAsyncResult AsyncResult = load.BeginInvoke(CallBack, null);
    }

    private bool CompareImages(string Dir, Bitmap InBit) {
      bool result = false;
      try {
        var Hash = HashImage(InBit);
        foreach(string file in Directory.GetFiles(Dir)) {
          if(!File.Exists(file))
            continue;

          using(var b = new Bitmap(file)) {
            if(result = Hash == HashImage(b))
              break;
          }
        }
      } catch {
        result = false;
      }

      return result;
    }

    private string HashImage(Bitmap InBit) {
      string result = string.Empty;
      try {
        using(var bit = new Bitmap(InBit, 8, 8)) {

          int buff = 0;
          List<byte> Bytes = new List<byte>();

          // градации серого
          for(int y = 0; y < bit.Height; y++) {
            for(int x = 0; x < bit.Width; x++) {
              var colorPixel = bit.GetPixel(x, y);
              int g = (int)((colorPixel.R * 0.2125) + (colorPixel.G * 0.7154) + (colorPixel.B * 0.0721));
              buff += g;
              bit.SetPixel(x, y, Color.FromArgb(colorPixel.A, g, g, g));
            }
          }

          var middle = buff / (bit.Height * bit.Width);
          for(int y = 0; y < bit.Height; y++) {
            for(int x = 0; x < bit.Width; x++) {
              var colorPixel = bit.GetPixel(x, y);
              int g = (int)((colorPixel.R * 0.2125) + (colorPixel.G * 0.7154) + (colorPixel.B * 0.0721));
              Bytes.Add((byte)(middle < g ? 0 : 1));
            }
          }

          var MD5Hash = MD5.Create();
          MD5Hash.ComputeHash(Bytes.ToArray());
          result = string.Join("", MD5Hash.Hash.Select(_ => _.ToString("X")));
        }
      } catch {
        result = string.Empty;
      }

      return result;
    }

    private List<string> LoadPageAndParce(int page) {
      var colorName = this.Invoke(new Func<string>(() => { return ItemToColor(); }));
      var typeName = this.Invoke(new Func<string>(() => { return ItemToType(); }));
      string url = string.Format(@"http://yandex.ru/images/search?p={0}&text={1}&icolor={2}&type={3}&isize=eq&iw=100&ih=100&itype=jpg&uinfo=sw-1920-sh-1080-ww-1903-wh-946-pd-1-wp-16x9_1920x1080", page, textBox1.Text, colorName, typeName);
      string html = string.Empty;
      List<string> ImgURL = new List<string>();
      try {
        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
        using(HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
          using(StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8)) {
            html = sr.ReadToEnd();
          }
        }
      } catch(Exception e) {
        MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        return ImgURL;
      }

      Regex newReg = new Regex("http://(.{3,200})(.jpg|.jpeg)", RegexOptions.IgnoreCase);
      var matches = newReg.Matches(html);
      foreach(Match match in matches) {
        var urlImg = match.Groups[0].Value;
        if(!ImgURL.Contains(urlImg))
          ImgURL.Add(urlImg);
      }

      return ImgURL;
    }

    private void textBox1_Enter(object sender, EventArgs e) {
      errorProvider1.Clear();
    }

    private void button2_Click(object sender, EventArgs e) {
      if(folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        toolStripStatusLabel1.Text = folderBrowserDialog1.SelectedPath;

      errorProvider1.Clear();
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
      progressBar1.Maximum = (int)numericUpDown1.Value;
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
      LoadImages();
    }


    private void LoadImages() {
      int idPage = 1;
      var imgUrl = new List<string>();
      const int ThreadCount = 10;

      // на 1 странице у нас 25 картинок
      var count = (int)Math.Ceiling((double)numericUpDown1.Value / 25);
      for(int i = 0; i < count; i++) {
        imgUrl.AddRange(LoadPageAndParce(idPage));
        idPage += 5;
      }

      AsyncCallback callBack = new AsyncCallback(Result => {
            //var rec = (List<string>)(Result as AsyncResult).AsyncDelegate;
        });

      List<IAsyncResult> resultMethods = new List<IAsyncResult>();
      var method = new DLoadAndSaveImag(LoadAndSaveImag);
      var tmp = new List<string>();
      for(int i = 0; i < imgUrl.Count(); i++) {
        if(backgroundWorker1.CancellationPending)
          break;
        
        tmp.Add(imgUrl[i]);
        if(i > 0 && i % ThreadCount == 0) {
          resultMethods.Add(method.BeginInvoke(tmp.ToArray(), callBack, null));
          tmp.Clear();
        } 
      }
      // если что-то осталось, используем
      if(tmp.Any()) {
        resultMethods.Add(method.BeginInvoke(tmp.ToArray(), callBack, null));
        tmp.Clear();
      }

      int workingThread = 0;
      while((workingThread = resultMethods.Count(_ => !_.IsCompleted)) > 0) {
        this.Invoke(new Action(() => Text = string.Format("Загрузка: Кол-во рабочих потоков {0}", workingThread)));
        Thread.Sleep(1000);
      }
    }


    private void LoadAndSaveImag(string[] imgUrl) {
      foreach(var url in imgUrl) {
        try {
          if(backgroundWorker1.CancellationPending)
            break;

          bool saved = false;
          HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
          using(HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
            using(var bit = new Bitmap(resp.GetResponseStream())) {
              var imgName = Path.GetFileName(url);
              var Dir     = (string)this.Invoke(new Func<string>(() => { return toolStripStatusLabel1.Text; }));
              var saveDir = Path.Combine(Dir, imgName);
              if(saved = (!CompareImages(Dir, bit) && bit.Height == 100 && bit.Width == 100)) // сохраняем размер только 100, т.к. именно такие и заказывали
                bit.Save(saveDir);
            }
          }

          if(saved) {
            this.Invoke(new Action(() => { if(progressBar1.Value < progressBar1.Maximum) progressBar1.Value++;  }));
            bool end = (bool)this.Invoke(new Func<bool>(() => { return progressBar1.Value >= numericUpDown1.Value; }));
            if(end)
              break;
          }

        } catch { }
      }
    }


    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      progressBar1.Value = 0;
      button2.Enabled = true;
      button1.Text = "Грузить";
      Text = "Загрузка:";
    }

    private void Form2_FormClosing(object sender, FormClosingEventArgs e) {
      if(button1.Text == "Прервать") {
        MessageBox.Show("Дождитесь окончания загрузки", "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        e.Cancel = true;
      }
      
    }

    private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {

    }

    private void button3_Click(object sender, EventArgs e) {
      colorDialog1.ShowDialog();
    }


    private void comboBox1_KeyPress(object sender, KeyPressEventArgs e) {
      e.Handled = true;
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
      ItemToColor();
    }

    private string ItemToType() {
      switch(comboBox2.SelectedIndex) {
        case 0:
          return "lineart";
        default:
          return null;
      }
    }

    private string ItemToColor() {
      comboBox1.ForeColor = Color.Black;

      switch(comboBox1.SelectedIndex) {
        case 0:
          comboBox1.BackColor = Color.Red;
          return "red";
        case 1:
          comboBox1.BackColor = Color.Green;
          comboBox1.ForeColor = Color.White;
          return "green";
        case 2:
          comboBox1.BackColor = Color.Blue;
          comboBox1.ForeColor = Color.White;
          return "blue";
        case 3:
          comboBox1.BackColor =Color.Yellow;
          return "yellow";
        case 4:
          comboBox1.BackColor = Color.Orange;
          return "orange";
        case 5:
          comboBox1.BackColor = Color.Violet;
          return "violet";
        case 6:
          comboBox1.BackColor = Color.Cyan;
          return "cyan";
        case 7:
          comboBox1.BackColor = Color.White;
          return "white";
        case 8:
          comboBox1.BackColor = Color.Black;
          comboBox1.ForeColor = Color.White;
          return "black";
        default:
          return null;
      }
    }
  }
}
