namespace WindowsFormsApplication6 {
  partial class Settings {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
      this.button1 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.button2 = new System.Windows.Forms.Button();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.trackBar2 = new System.Windows.Forms.TrackBar();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(212, 23);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(33, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "...";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(15, 25);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(191, 20);
      this.textBox1.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(164, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Путь для сохранения картинки";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 54);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(200, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Директория с картинками (исходные)";
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(15, 70);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(191, 20);
      this.textBox2.TabIndex = 4;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(212, 68);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(33, 23);
      this.button2.TabIndex = 3;
      this.button2.Text = "...";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // trackBar1
      // 
      this.trackBar1.Location = new System.Drawing.Point(15, 112);
      this.trackBar1.Minimum = 1;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(191, 45);
      this.trackBar1.TabIndex = 6;
      this.trackBar1.Value = 1;
      this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 96);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(176, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Вероятность разброса картинок:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 137);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(192, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Ораничение % соответствия цветов:";
      // 
      // trackBar2
      // 
      this.trackBar2.Location = new System.Drawing.Point(15, 149);
      this.trackBar2.Maximum = 100;
      this.trackBar2.Minimum = 1;
      this.trackBar2.Name = "trackBar2";
      this.trackBar2.Size = new System.Drawing.Size(191, 45);
      this.trackBar2.TabIndex = 8;
      this.trackBar2.Value = 50;
      this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
      // 
      // Settings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(258, 184);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.trackBar2);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.trackBar1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textBox2);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "Settings";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Settings";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
      this.Load += new System.EventHandler(this.Settings_Load);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TrackBar trackBar2;
  }
}