namespace DLPPriter.View
{
    partial class ProjectorFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_poweron = new System.Windows.Forms.Button();
            this.bt_poweroff = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_ledonoff = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.tb_currt = new System.Windows.Forms.TextBox();
            this.bt_writercurrt = new System.Windows.Forms.Button();
            this.bt_readercurrt = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.check_x = new System.Windows.Forms.CheckBox();
            this.check_y = new System.Windows.Forms.CheckBox();
            this.check_xy = new System.Windows.Forms.CheckBox();
            this.bt_xyfilter = new System.Windows.Forms.Button();
            this.bt_load = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bt_poweroff);
            this.panel1.Controls.Add(this.bt_poweron);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 66);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "电源控制:";
            // 
            // bt_poweron
            // 
            this.bt_poweron.Location = new System.Drawing.Point(139, 12);
            this.bt_poweron.Name = "bt_poweron";
            this.bt_poweron.Size = new System.Drawing.Size(79, 34);
            this.bt_poweron.TabIndex = 1;
            this.bt_poweron.Text = "开";
            this.bt_poweron.UseVisualStyleBackColor = true;
            this.bt_poweron.Click += new System.EventHandler(this.bt_poweron_Click);
            // 
            // bt_poweroff
            // 
            this.bt_poweroff.Location = new System.Drawing.Point(253, 12);
            this.bt_poweroff.Name = "bt_poweroff";
            this.bt_poweroff.Size = new System.Drawing.Size(79, 34);
            this.bt_poweroff.TabIndex = 2;
            this.bt_poweroff.Text = "关";
            this.bt_poweroff.UseVisualStyleBackColor = true;
            this.bt_poweroff.Click += new System.EventHandler(this.bt_poweroff_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bt_ledonoff);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(413, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(303, 66);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "LED灯控制：";
            // 
            // bt_ledonoff
            // 
            this.bt_ledonoff.Location = new System.Drawing.Point(179, 12);
            this.bt_ledonoff.Name = "bt_ledonoff";
            this.bt_ledonoff.Size = new System.Drawing.Size(79, 34);
            this.bt_ledonoff.TabIndex = 3;
            this.bt_ledonoff.Text = "开";
            this.bt_ledonoff.UseVisualStyleBackColor = true;
            this.bt_ledonoff.Click += new System.EventHandler(this.bt_ledonoff_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bt_readercurrt);
            this.groupBox1.Controls.Add(this.bt_writercurrt);
            this.groupBox1.Controls.Add(this.tb_currt);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(704, 116);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LED电流控制";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 27);
            this.label3.TabIndex = 0;
            this.label3.Text = "LED:";
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(86, 50);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(331, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // tb_currt
            // 
            this.tb_currt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_currt.Location = new System.Drawing.Point(423, 47);
            this.tb_currt.Name = "tb_currt";
            this.tb_currt.Size = new System.Drawing.Size(88, 34);
            this.tb_currt.TabIndex = 2;
            this.tb_currt.TextChanged += new System.EventHandler(this.tb_currt_TextChanged);
            // 
            // bt_writercurrt
            // 
            this.bt_writercurrt.Location = new System.Drawing.Point(527, 45);
            this.bt_writercurrt.Name = "bt_writercurrt";
            this.bt_writercurrt.Size = new System.Drawing.Size(71, 34);
            this.bt_writercurrt.TabIndex = 3;
            this.bt_writercurrt.Text = "写";
            this.bt_writercurrt.UseVisualStyleBackColor = true;
            this.bt_writercurrt.Click += new System.EventHandler(this.bt_writercurrt_Click);
            // 
            // bt_readercurrt
            // 
            this.bt_readercurrt.Location = new System.Drawing.Point(616, 45);
            this.bt_readercurrt.Name = "bt_readercurrt";
            this.bt_readercurrt.Size = new System.Drawing.Size(71, 34);
            this.bt_readercurrt.TabIndex = 4;
            this.bt_readercurrt.Text = "读";
            this.bt_readercurrt.UseVisualStyleBackColor = true;
            this.bt_readercurrt.Click += new System.EventHandler(this.bt_readercurrt_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_xyfilter);
            this.groupBox2.Controls.Add(this.check_xy);
            this.groupBox2.Controls.Add(this.check_y);
            this.groupBox2.Controls.Add(this.check_x);
            this.groupBox2.Location = new System.Drawing.Point(12, 206);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(704, 85);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "图像镜像";
            // 
            // check_x
            // 
            this.check_x.AutoSize = true;
            this.check_x.Location = new System.Drawing.Point(8, 33);
            this.check_x.Name = "check_x";
            this.check_x.Size = new System.Drawing.Size(111, 31);
            this.check_x.TabIndex = 0;
            this.check_x.Text = "左右镜像";
            this.check_x.UseVisualStyleBackColor = true;
            this.check_x.CheckedChanged += new System.EventHandler(this.check_x_CheckedChanged);
            // 
            // check_y
            // 
            this.check_y.AutoSize = true;
            this.check_y.Location = new System.Drawing.Point(158, 33);
            this.check_y.Name = "check_y";
            this.check_y.Size = new System.Drawing.Size(111, 31);
            this.check_y.TabIndex = 1;
            this.check_y.Text = "上下镜像";
            this.check_y.UseVisualStyleBackColor = true;
            this.check_y.CheckedChanged += new System.EventHandler(this.check_y_CheckedChanged);
            // 
            // check_xy
            // 
            this.check_xy.AutoSize = true;
            this.check_xy.Location = new System.Drawing.Point(319, 33);
            this.check_xy.Name = "check_xy";
            this.check_xy.Size = new System.Drawing.Size(151, 31);
            this.check_xy.TabIndex = 2;
            this.check_xy.Text = "左右上下镜像";
            this.check_xy.UseVisualStyleBackColor = true;
            this.check_xy.CheckedChanged += new System.EventHandler(this.check_xy_CheckedChanged);
            // 
            // bt_xyfilter
            // 
            this.bt_xyfilter.Location = new System.Drawing.Point(562, 30);
            this.bt_xyfilter.Name = "bt_xyfilter";
            this.bt_xyfilter.Size = new System.Drawing.Size(71, 34);
            this.bt_xyfilter.TabIndex = 5;
            this.bt_xyfilter.Text = "执行";
            this.bt_xyfilter.UseVisualStyleBackColor = true;
            this.bt_xyfilter.Click += new System.EventHandler(this.bt_xyfilter_Click);
            // 
            // bt_load
            // 
            this.bt_load.Location = new System.Drawing.Point(12, 310);
            this.bt_load.Name = "bt_load";
            this.bt_load.Size = new System.Drawing.Size(128, 46);
            this.bt_load.TabIndex = 4;
            this.bt_load.Text = "加载图片";
            this.bt_load.UseVisualStyleBackColor = true;
            this.bt_load.Click += new System.EventHandler(this.bt_load_Click);
            // 
            // ProjectorFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 592);
            this.Controls.Add(this.bt_load);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "ProjectorFrom";
            this.Text = "ProjectorFrom";
            this.Load += new System.EventHandler(this.ProjectorFrom_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bt_poweron;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_poweroff;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_ledonoff;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bt_readercurrt;
        private System.Windows.Forms.Button bt_writercurrt;
        private System.Windows.Forms.TextBox tb_currt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox check_y;
        private System.Windows.Forms.CheckBox check_x;
        private System.Windows.Forms.Button bt_xyfilter;
        private System.Windows.Forms.CheckBox check_xy;
        private System.Windows.Forms.Button bt_load;
    }
}