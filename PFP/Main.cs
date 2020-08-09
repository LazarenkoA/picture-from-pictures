using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Xml.Serialization;

namespace WindowsFormsApplication6 {
  public partial class Main:Form {
    private delegate Bitmap DJoinAndSaveImg(string SelectedPath);
    public Configuration conf;
    public Random random;

    [Serializable]
    public class Configuration {
      public string FolderImgPath;
      public string ImgSavePath;
      public byte RoundTo ;
      public byte LimitPercent;

      public Configuration() {
        RoundTo = 1;
        LimitPercent = 50;
      }
    }

    private class GridData {
      internal Color color;
      internal string imgPath;
      internal int x;
      internal int y;
      internal int width;
      internal int height;

      public GridData(Rectangle Rec) {
        x      = Rec.X;
        y      = Rec.Y;
        width  = Rec.Width;
        height = Rec.Height;

        imgPath = string.Empty;
      }
    }

    List<GridData> Grid;
    int SourceHeight;
    int SourceWidth;

    public Main() {
      InitializeComponent();
      Grid = new List<GridData>();
      random = new Random();
      RestoreSettings();
    }

    private void button1_Click(object sender, EventArgs e) {
      openFileDialog1.Filter = "Картинки|(*.BMP;*.JPG;*.JPEG;*.GIF)";
      if(openFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
        return;

      using(var Bit = new Bitmap(openFileDialog1.FileName)) {
        SourceHeight = Bit.Height;
        SourceWidth  = Bit.Width;
        var coeff    = (double)SourceHeight / (double)SourceWidth;

        using(var bitSmall = new Bitmap(Bit, pictureBox1.Width, (int)(pictureBox1.Height * coeff))) {
          pictureBox1.Image = bitSmall.Clone(new Rectangle(0, 0, bitSmall.Width, bitSmall.Height), bitSmall.PixelFormat);
        }

        trackBar1.Value = 1;
        trackBar1.Maximum = Math.Max(pictureBox1.Image.Height, pictureBox1.Image.Width) / 5;
      }

      //using(var img = Image.FromFile(openFileDialog1.FileName)) {
      //  using(var bit = new Bitmap(img, pictureBox1.Width, pictureBox1.Height)) {
      //    pictureBox1.Image = bit.Clone(new Rectangle(0, 0, bit.Width, bit.Height), bit.PixelFormat);
      //  }
      //}


      //using(var bit = new Bitmap(Image.FromFile(openFileDialog1.FileName), pictureBox2.Width, pictureBox2.Height)) {
      //  using(Graphics myGraphics = pictureBox3.CreateGraphics()) {
      //    Rectangle fillRect = new Rectangle(0, 0, bit.Width, bit.Height);
      //    Region rect = new Region(fillRect);
      //    var old = RColor;

      //    using(SolidBrush myHatchBrush = new SolidBrush(RColor = MixColor(bit))) {
      //      myGraphics.FillRegion(myHatchBrush, rect);
      //    }

      //    //var _R = (double)Math.Min(old.R, RColor.R) / (double)Math.Max(old.R, RColor.R) * 100;
      //    //var _G = (double)Math.Min(old.G, RColor.G) / (double)Math.Max(old.G, RColor.G) * 100;
      //    //var _B = (double)Math.Min(old.B, RColor.B) / (double)Math.Max(old.B, RColor.B) * 100;

      //    //Text = "";
      //    //Text += ((_R + _G + _B) / 3).ToString();

      //  }
      //}

   }

    private double GetPercent(Color Color1, Color Color2) {
      var _R = (double)Math.Min(Color1.R, Color2.R) / (double)Math.Max(Color1.R, Color2.R) * 100;
      var _G = (double)Math.Min(Color1.G, Color2.G) / (double)Math.Max(Color1.G, Color2.G) * 100;
      var _B = (double)Math.Min(Color1.B, Color2.B) / (double)Math.Max(Color1.B, Color2.B) * 100;

      return (_R + _G + _B) / 3;
    }


    private Color MixColor(Bitmap bit) {
      int CommonR = 0, CommonG = 0, CommonB = 0;
      using(bit) {
       // bit.SetResolution(pictureBox1.Width, pictureBox1.Height);
        for(int y = 0; y < bit.Height; y++) {
          for(int x = 0; x < bit.Width; x++) {
            var colorPixel = bit.GetPixel(x, y);
            CommonR += colorPixel.R;
            CommonG += colorPixel.G;
            CommonB += colorPixel.B;
          }
        }

        var size = bit.Height * bit.Width;
        return Color.FromArgb(CommonR / size, CommonG / size, CommonB / size);
      }
    }
    

    

    private void button2_Click(object sender, EventArgs e) {
      if(pictureBox1.Image == null) {
        MessageBox.Show("Нужно выбрать обрабатываемую картинку", "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        return;
      }

      folderBrowserDialog1.Description = "Укажите директорию с картинками";
      if(string.IsNullOrEmpty(conf.FolderImgPath)) {
        if(folderBrowserDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
          return;
        conf.FolderImgPath = folderBrowserDialog1.SelectedPath;
      }

      saveFileDialog1.Filter = "*.jpg|*.jpg";
      if(string.IsNullOrEmpty(conf.ImgSavePath)) {
        if(saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          return;
        conf.ImgSavePath = saveFileDialog1.FileName;
      } else if(File.Exists(conf.ImgSavePath)) {
        if(MessageBox.Show(string.Format("Файл\r\n{0}\r\nсуществует, заменить его?", conf.ImgSavePath), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) == System.Windows.Forms.DialogResult.No) {
          if(saveFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            return;
          conf.ImgSavePath = saveFileDialog1.FileName;
        }
      }

      button1.Enabled    = false;
      button2.Enabled    = false;

      //Action<string> AsyncMethod = SelectedPath => JoinAndSaveImg(SelectedPath);
      AsyncCallback AsyncCallBackMethod = new AsyncCallback(Result => {
                                                            DJoinAndSaveImg @delegate = (DJoinAndSaveImg)(Result as AsyncResult).AsyncDelegate;
                                                            Bitmap bitOut             = @delegate.EndInvoke(Result);
                                                            bitOut.Save(conf.ImgSavePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                                            
                                                            this.Invoke(new Action(() => { toolStripProgressBar1.Value = 0; }));
                                                            this.Invoke(new Action(() => { toolStripStatusLabel1.Text = "Готово"; }));
                                                            this.Invoke(new Action(() => { button1.Enabled = true; button2.Enabled = true; }));
                                                          });

      var AsyncMethod = new DJoinAndSaveImg(JoinAndSaveImg);
      AsyncMethod.BeginInvoke(conf.FolderImgPath, AsyncCallBackMethod, null);
      
    }


    private Bitmap JoinAndSaveImg(string ImgPath) {
      if(!Directory.Exists(ImgPath))
        return null;

      this.Invoke(new Action(() => { toolStripStatusLabel1.Text = "Загружаем картинки"; }));
      this.Invoke(new Action(() => { toolStripProgressBar1.Value = 0; }));
      List<Tuple<Color, string>> Colors = new List<Tuple<Color, string>>();
      var Files = Directory.GetFiles(ImgPath, "*.jpg", SearchOption.AllDirectories);
      var Count = Files.Count();

      for(int i = 0; i < Count; i++) {
        using(var img = Image.FromFile(Files[i])) {
          using(var d = new Bitmap(img, 100, 100)) {
            Colors.Add(new Tuple<Color, string>(MixColor(d), Files[i]));
          }
        }
        double coeff = (double)i / (double)Count;
        this.Invoke(new Action(() => { toolStripProgressBar1.Value = (int)(toolStripProgressBar1.Maximum * coeff); }));
      }

      Func<int, int> RoundTo = In => {
        int Mod = 0;
        if((Mod = In % conf.RoundTo) != 0)
          return In - Mod;
        else
          return In;
      };

      this.Invoke(new Action(() => { toolStripProgressBar1.Value = 0; }));
      this.Invoke(new Action(() => { toolStripStatusLabel1.Text = "Определяем соотношение цветов"; }));
      Count = Grid.Count();
      for(int i = 0; i < Count; i++) {
        var SuccesColor = Colors.Select(_ => new { ImgPath = _.Item2, percent = (int)GetPercent(Grid[i].color, _.Item1) })
                                .Where(_ => _.percent >= conf.LimitPercent)
                                .GroupBy(_ => RoundTo(_.percent))
                                .OrderBy(_ => _.Key).LastOrDefault();

        if(SuccesColor != null && SuccesColor.Any()) {
          var randomItem = random.Next(SuccesColor.Count());
          Grid[i].imgPath = SuccesColor.ElementAt(randomItem).ImgPath;
        } else
          Grid[i].imgPath = string.Empty;

        double coeff = (double)i / (double)Count;
        this.Invoke(new Action(() => { toolStripProgressBar1.Value = (int)(toolStripProgressBar1.Maximum * coeff); }));
      }

      this.Invoke(new Action(() => { toolStripProgressBar1.Value = 0; }));
      this.Invoke(new Action(() => { toolStripStatusLabel1.Text = "Собираем результат"; }));

      #region Сохранение
      var bitOut = new Bitmap(SourceWidth, SourceHeight);
      //using(var bitOut = new Bitmap(SaveBit.Width, SaveBit.Height)) {
      using(Graphics g = Graphics.FromImage(bitOut)) {
        using(SolidBrush myHatchBrush = new SolidBrush(Color.Black)) {
          for(int i = 0; i < Count; i++) {
            // коэфециент для разницы размеров
            var coeffWidth = (double)Grid[i].width / (double)pictureBox1.Image.Width;
            var coeffHeight = (double)Grid[i].height / (double)pictureBox1.Image.Height;
            var coeffTop = (double)Grid[i].y / (double)pictureBox1.Image.Height;
            var coeffLeft = (double)Grid[i].x / (double)pictureBox1.Image.Width;

            var cellWidth = (int)Math.Ceiling(SourceWidth * coeffWidth);
            var cellHeight = (int)Math.Ceiling(SourceHeight * coeffHeight);
            var cellTop = (int)Math.Ceiling(SourceHeight * coeffTop);
            var cellLeft = (int)Math.Ceiling(SourceWidth * coeffLeft);

            var rec = new Rectangle(cellLeft, cellTop, cellWidth, cellHeight);

            if(string.IsNullOrEmpty(Grid[i].imgPath) || !File.Exists(Grid[i].imgPath)) {
              myHatchBrush.Color = Grid[i].color;
              using(Region rect = new Region(rec)) {
                g.FillRegion(myHatchBrush, rect);
              }
              continue;
            }


            using(var img = Image.FromFile(Grid[i].imgPath)) {
              using(var bitIn = new Bitmap(img, cellWidth, cellHeight)) {
                g.DrawImage(bitIn, rec);
              }
            }

            double coeff = (double)i / (double)Count;
            this.Invoke(new Action(() => { toolStripProgressBar1.Value = (int)(toolStripProgressBar1.Maximum * coeff); }));
          }
        } // Graphics

      #endregion

        return bitOut;

        #region Отображение в pictureBox
        //using(Graphics g2 = pictureBox2.CreateGraphics()) {
        //  foreach(var cell in Grid) {
        //    var rec = new Rectangle(cell.x, cell.y, cell.width, cell.height);

        //    if(string.IsNullOrEmpty(cell.imgPath)) {
        //      using(SolidBrush myHatchBrush = new SolidBrush(cell.color)) {
        //        using(Region rect = new Region(rec)) {
        //          g2.FillRegion(myHatchBrush, rect);
        //        }
        //      }
        //      continue;
        //    }


        //    using(var img = Image.FromFile(cell.imgPath)) {
        //      using(var d = new Bitmap(img, pictureBox2.Width, pictureBox2.Height)) {
        //        g2.DrawImage(d, rec, rec, GraphicsUnit.Pixel);
        //      }
        //    }
        //  }

        //}
        #endregion
      }
    }


    private void ToolStripMenuItem_Click(object sender, EventArgs e) {
      var form2 = new Load();
      form2.ShowDialog();
    }


    private void PositionLoadImg() {
      pictureBox1.Left = (this.Width / 2) - (pictureBox1.Width / 2);
      pictureBox1.Top = (this.Height / 2) - (pictureBox1.Height / 2);
    }

    private void Form1_Resize(object sender, EventArgs e) {
      PositionLoadImg();
    }


    private void PaintGrid() {
      if(pictureBox1.Image == null)
        return;

      using(Graphics g = pictureBox1.CreateGraphics()) {
        using(Pen p = new Pen(Color.Black)) {
          //p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

          pictureBox1.Refresh();
          //pictureBox2.Refresh();

          // размер ячейки
          var cellSize = Math.Max(pictureBox1.Image.Height, pictureBox1.Image.Width) / trackBar1.Value;
          var columns  = Math.Max(pictureBox1.Image.Height, pictureBox1.Image.Width) / cellSize;
          Grid.Clear();


          using(var b = new Bitmap(pictureBox1.Image)) {
            for(int i = 0; i < columns; i++) {
              g.DrawLine(p, new Point(i * cellSize, 0), new Point(i * cellSize, pictureBox1.Height));
              g.DrawLine(p, new Point(0, i * cellSize), new Point(pictureBox1.Width, i * cellSize));

              var ImageRec = new Rectangle(0, 0, b.Width, b.Height);
              for(int j = 0; j < columns; j++) {
                var Rec = new Rectangle(i * cellSize, j * cellSize, cellSize, cellSize);
                var data = new GridData(Rec);
                Rec.Intersect(ImageRec);
                if(!Rec.IsEmpty && Rec.Height > 0 && Rec.Width > 0)
                  data.color = MixColor(b.Clone(Rec, b.PixelFormat));
                Grid.Add(data);



                //var color = MixColor(d.Clone(new Rectangle(i * cell, j * cell, cell, cell), System.Drawing.Imaging.PixelFormat.Format32bppRgb));
                //using(SolidBrush myHatchBrush = new SolidBrush(color)) {
                //  Rectangle fillRect = new Rectangle(i * cell, j * cell, cell, cell);
                //  using(Region rect = new Region(fillRect)) {
                //    g2.FillRegion(myHatchBrush, rect);
                //  }
                //}

              }
            }
          }
        }
      }
    }




    private void Form1_Paint(object sender, PaintEventArgs e) {
      if(Grid == null || !Grid.Any())
        return;

      pictureBox1.Refresh();
      using(Graphics g = pictureBox1.CreateGraphics()) {
        using(Pen p = new Pen(Color.Black)) {
          var cellSize = Grid.First().width;
          var columns = pictureBox1.Width / Grid.First().width;

          for(int i = 0; i < columns; i++) {
            g.DrawLine(p, new Point(i * cellSize, 0), new Point(i * cellSize, pictureBox1.Height));
            g.DrawLine(p, new Point(0, i * cellSize), new Point(pictureBox1.Width, i * cellSize));
          }
        }
      }
    }

    private void Form1_Load(object sender, EventArgs e) {
      PositionLoadImg();
    }

    private void настройкиToolStripMenuItem_Click(object sender, EventArgs e) {
      var form3 = new Settings();
      form3.ShowDialog(this);
    }


    private void SaveSettings() {
      string SettingsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.xml");
      var serializer = new XmlSerializer(typeof(Configuration));
      using(var stream = new FileStream(SettingsPath, FileMode.Create, FileAccess.Write)) {
        serializer.Serialize(stream, conf);
      }
    }

    private void RestoreSettings() {
      string SettingsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Settings.xml");
      if(!File.Exists(SettingsPath)) {
        conf = new Configuration() { RoundTo = 1 };
        return;
      }

      var serializer = new XmlSerializer(typeof(Configuration));

      string[] Data = File.ReadAllLines(SettingsPath);
      using(var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(string.Join("\r\n", Data)))) {
        conf = (Configuration)serializer.Deserialize(stream);
      }
    }


    private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
      SaveSettings();
    }

    private void trackBar1_ValueChanged(object sender, EventArgs e) {
      PaintGrid();
    }



  }
}
