namespace DLPPriter.View
{
    partial class AutoFrom
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bt_run = new System.Windows.Forms.Button();
            this.bt_pance = new System.Windows.Forms.Button();
            this.bt_stop = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1056, 776);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1056, 776);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // bt_run
            // 
            this.bt_run.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.bt_run.FlatAppearance.BorderSize = 0;
            this.bt_run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_run.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_run.ForeColor = System.Drawing.Color.White;
            this.bt_run.Location = new System.Drawing.Point(1100, 158);
            this.bt_run.Name = "bt_run";
            this.bt_run.Size = new System.Drawing.Size(150, 50);
            this.bt_run.TabIndex = 4;
            this.bt_run.Text = "运行";
            this.bt_run.UseVisualStyleBackColor = false;
            this.bt_run.Click += new System.EventHandler(this.bt_run_Click);
            // 
            // bt_pance
            // 
            this.bt_pance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.bt_pance.FlatAppearance.BorderSize = 0;
            this.bt_pance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_pance.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_pance.ForeColor = System.Drawing.Color.White;
            this.bt_pance.Location = new System.Drawing.Point(1100, 377);
            this.bt_pance.Name = "bt_pance";
            this.bt_pance.Size = new System.Drawing.Size(150, 50);
            this.bt_pance.TabIndex = 5;
            this.bt_pance.Text = "暂停";
            this.bt_pance.UseVisualStyleBackColor = false;
            this.bt_pance.Click += new System.EventHandler(this.bt_pance_Click);
            // 
            // bt_stop
            // 
            this.bt_stop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.bt_stop.FlatAppearance.BorderSize = 0;
            this.bt_stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_stop.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_stop.ForeColor = System.Drawing.Color.White;
            this.bt_stop.Location = new System.Drawing.Point(1100, 614);
            this.bt_stop.Name = "bt_stop";
            this.bt_stop.Size = new System.Drawing.Size(150, 50);
            this.bt_stop.TabIndex = 6;
            this.bt_stop.Text = "停止";
            this.bt_stop.UseVisualStyleBackColor = false;
            this.bt_stop.Click += new System.EventHandler(this.bt_stop_Click);
            // 
            // AutoFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 800);
            this.Controls.Add(this.bt_stop);
            this.Controls.Add(this.bt_pance);
            this.Controls.Add(this.bt_run);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "AutoFrom";
            this.Text = "DebugFrom";
            this.Load += new System.EventHandler(this.AutoFrom_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bt_run;
        private System.Windows.Forms.Button bt_pance;
        private System.Windows.Forms.Button bt_stop;
    }
}