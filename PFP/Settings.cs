using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication6 {
  public partial class Settings:Form {
    public Settings() {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e) {
      saveFileDialog1.Filter = "*.jpg|*.jpg";
      if(saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return;

      textBox1.Text = (Owner as Main).conf.ImgSavePath = saveFileDialog1.FileName;
    }

    private void button2_Click(object sender, EventArgs e) {
      folderBrowserDialog1.Description = "Выберите директорию с картинками из которых будет выложена картинка";
      if(folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return;

      textBox2.Text = (Owner as Main).conf.FolderImgPath = folderBrowserDialog1.SelectedPath;
    }

    private void Settings_FormClosing(object sender, FormClosingEventArgs e) {

      if(!Directory.Exists(textBox2.Text) && !string.IsNullOrEmpty(textBox2.Text)) {
        e.Cancel = true;
        MessageBox.Show(string.Format("Директория\r\n{0}\r\nНе существует", textBox2.Text), "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
      }

      if(!string.IsNullOrEmpty(textBox1.Text)) {
        var SaveFileDir = Path.GetDirectoryName(textBox1.Text);
        if(!Directory.Exists(SaveFileDir)) {
          e.Cancel = true;
          MessageBox.Show(string.Format("Директория\r\n{0}\r\nНе существует", SaveFileDir), "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
      }
    }

    private void Settings_Load(object sender, EventArgs e) {
      textBox1.Text   = (Owner as Main).conf.ImgSavePath;
      textBox2.Text   = (Owner as Main).conf.FolderImgPath;
      trackBar1.Value = (Owner as Main).conf.RoundTo;
      trackBar2.Value = (Owner as Main).conf.LimitPercent;
    }

    private void trackBar1_ValueChanged(object sender, EventArgs e) {
      label3.Text = string.Format("Вероятность разброса картинок: {0}", trackBar1.Value);
      (Owner as Main).conf.RoundTo = (byte)trackBar1.Value;
    }


    private void trackBar2_ValueChanged(object sender, EventArgs e) {
      label4.Text = string.Format("Ораничение % соответствия цветов: {0}", trackBar2.Value);
      (Owner as Main).conf.LimitPercent = (byte)trackBar2.Value;
    }
  }
}
