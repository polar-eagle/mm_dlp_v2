namespace DLPPriter.View
{
    partial class MotionStationControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_axisname = new System.Windows.Forms.Label();
            this.uCircleButton1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_clearerror = new System.Windows.Forms.Button();
            this.lb_pos = new System.Windows.Forms.Label();
            this.tb_acc = new System.Windows.Forms.RichTextBox();
            this.tb_speed = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.uCircleButton2 = new System.Windows.Forms.Button();
            this.lb_current = new System.Windows.Forms.Label();
            this.lb_voltage = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_dec = new System.Windows.Forms.RichTextBox();
            this.bt_absmove = new System.Windows.Forms.Button();
            this.tb_position = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tb_position);
            this.panel1.Controls.Add(this.bt_absmove);
            this.panel1.Controls.Add(this.tb_dec);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lb_voltage);
            this.panel1.Controls.Add(this.lb_current);
            this.panel1.Controls.Add(this.uCircleButton2);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.tb_speed);
            this.panel1.Controls.Add(this.tb_acc);
            this.panel1.Controls.Add(this.lb_pos);
            this.panel1.Controls.Add(this.bt_clearerror);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.uCircleButton1);
            this.panel1.Controls.Add(this.lb_axisname);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 254);
            this.panel1.TabIndex = 0;
            // 
            // lb_axisname
            // 
            this.lb_axisname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lb_axisname.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_axisname.ForeColor = System.Drawing.Color.White;
            this.lb_axisname.Location = new System.Drawing.Point(8, 3);
            this.lb_axisname.Name = "lb_axisname";
            this.lb_axisname.Size = new System.Drawing.Size(150, 39);
            this.lb_axisname.TabIndex = 0;
            this.lb_axisname.Text = "label1";
            this.lb_axisname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb_axisname.MouseEnter += new System.EventHandler(this.lb_axisname_MouseEnter);
            // 
            // uCircleButton1
            // 
            this.uCircleButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.uCircleButton1.FlatAppearance.BorderSize = 0;
            this.uCircleButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uCircleButton1.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uCircleButton1.ForeColor = System.Drawing.Color.White;
            this.uCircleButton1.Location = new System.Drawing.Point(8, 67);
            this.uCircleButton1.Name = "uCircleButton1";
            this.uCircleButton1.Size = new System.Drawing.Size(150, 39);
            this.uCircleButton1.TabIndex = 1;
            this.uCircleButton1.Text = "使能";
            this.uCircleButton1.UseVisualStyleBackColor = false;
            this.uCircleButton1.Click += new System.EventHandler(this.bt_motor_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(8, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 39);
            this.label2.TabIndex = 2;
            this.label2.Text = "加速度：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.label3.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(8, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 39);
            this.label3.TabIndex = 3;
            this.label3.Text = "速度：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bt_clearerror
            // 
            this.bt_clearerror.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.bt_clearerror.FlatAppearance.BorderSize = 0;
            this.bt_clearerror.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_clearerror.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_clearerror.ForeColor = System.Drawing.Color.White;
            this.bt_clearerror.Location = new System.Drawing.Point(164, 3);
            this.bt_clearerror.Name = "bt_clearerror";
            this.bt_clearerror.Size = new System.Drawing.Size(150, 39);
            this.bt_clearerror.TabIndex = 4;
            this.bt_clearerror.Text = "清除异常";
            this.bt_clearerror.UseVisualStyleBackColor = false;
            this.bt_clearerror.Click += new System.EventHandler(this.bt_clearerror_Click);
            // 
            // lb_pos
            // 
            this.lb_pos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lb_pos.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_pos.ForeColor = System.Drawing.Color.White;
            this.lb_pos.Location = new System.Drawing.Point(164, 67);
            this.lb_pos.Name = "lb_pos";
            this.lb_pos.Size = new System.Drawing.Size(150, 39);
            this.lb_pos.TabIndex = 5;
            this.lb_pos.Text = "1200.0000";
            this.lb_pos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tb_acc
            // 
            this.tb_acc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tb_acc.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_acc.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_acc.ForeColor = System.Drawing.Color.White;
            this.tb_acc.Location = new System.Drawing.Point(164, 130);
            this.tb_acc.Name = "tb_acc";
            this.tb_acc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tb_acc.Size = new System.Drawing.Size(150, 39);
            this.tb_acc.TabIndex = 6;
            this.tb_acc.Text = "100";
            this.tb_acc.Leave += new System.EventHandler(this.tb_acc_TextChanged);
            // 
            // tb_speed
            // 
            this.tb_speed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tb_speed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_speed.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_speed.ForeColor = System.Drawing.Color.White;
            this.tb_speed.Location = new System.Drawing.Point(164, 191);
            this.tb_speed.Name = "tb_speed";
            this.tb_speed.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tb_speed.Size = new System.Drawing.Size(150, 39);
            this.tb_speed.TabIndex = 7;
            this.tb_speed.Text = "1000";
            this.tb_speed.Leave += new System.EventHandler(this.tb_speed_TextChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(320, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 39);
            this.button2.TabIndex = 8;
            this.button2.Text = "停止移动";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.bt_stopmove_Click);
            // 
            // uCircleButton2
            // 
            this.uCircleButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.uCircleButton2.FlatAppearance.BorderSize = 0;
            this.uCircleButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uCircleButton2.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uCircleButton2.ForeColor = System.Drawing.Color.White;
            this.uCircleButton2.Location = new System.Drawing.Point(476, 3);
            this.uCircleButton2.Name = "uCircleButton2";
            this.uCircleButton2.Size = new System.Drawing.Size(150, 39);
            this.uCircleButton2.TabIndex = 9;
            this.uCircleButton2.Text = "设定原点";
            this.uCircleButton2.UseVisualStyleBackColor = false;
            this.uCircleButton2.Click += new System.EventHandler(this.uCircleButton2_Click);
            // 
            // lb_current
            // 
            this.lb_current.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lb_current.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_current.ForeColor = System.Drawing.Color.White;
            this.lb_current.Location = new System.Drawing.Point(320, 67);
            this.lb_current.Name = "lb_current";
            this.lb_current.Size = new System.Drawing.Size(150, 39);
            this.lb_current.TabIndex = 10;
            this.lb_current.Text = "10.00A";
            this.lb_current.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_voltage
            // 
            this.lb_voltage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lb_voltage.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_voltage.ForeColor = System.Drawing.Color.White;
            this.lb_voltage.Location = new System.Drawing.Point(476, 67);
            this.lb_voltage.Name = "lb_voltage";
            this.lb_voltage.Size = new System.Drawing.Size(150, 39);
            this.lb_voltage.TabIndex = 11;
            this.lb_voltage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.label4.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(320, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 39);
            this.label4.TabIndex = 12;
            this.label4.Text = "减速度：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tb_dec
            // 
            this.tb_dec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tb_dec.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_dec.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_dec.ForeColor = System.Drawing.Color.White;
            this.tb_dec.Location = new System.Drawing.Point(476, 130);
            this.tb_dec.Name = "tb_dec";
            this.tb_dec.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tb_dec.Size = new System.Drawing.Size(150, 39);
            this.tb_dec.TabIndex = 13;
            this.tb_dec.Text = "-100";
            this.tb_dec.Leave += new System.EventHandler(this.tb_dec_TextChanged);
            // 
            // bt_absmove
            // 
            this.bt_absmove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.bt_absmove.FlatAppearance.BorderSize = 0;
            this.bt_absmove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_absmove.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_absmove.ForeColor = System.Drawing.Color.White;
            this.bt_absmove.Location = new System.Drawing.Point(320, 191);
            this.bt_absmove.Name = "bt_absmove";
            this.bt_absmove.Size = new System.Drawing.Size(150, 39);
            this.bt_absmove.TabIndex = 14;
            this.bt_absmove.Text = "设定位置";
            this.bt_absmove.UseVisualStyleBackColor = false;
            this.bt_absmove.Click += new System.EventHandler(this.bt_absmove_Click);
            // 
            // tb_position
            // 
            this.tb_position.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tb_position.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_position.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_position.ForeColor = System.Drawing.Color.White;
            this.tb_position.Location = new System.Drawing.Point(476, 191);
            this.tb_position.Name = "tb_position";
            this.tb_position.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.tb_position.Size = new System.Drawing.Size(150, 39);
            this.tb_position.TabIndex = 15;
            this.tb_position.Text = "1";
            // 
            // MotionStationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "MotionStationControl";
            this.Size = new System.Drawing.Size(631, 254);
            this.Load += new System.EventHandler(this.MotionStationControl_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button uCircleButton1;
        private System.Windows.Forms.Label lb_pos;
        private System.Windows.Forms.Button bt_clearerror;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox tb_speed;
        private System.Windows.Forms.RichTextBox tb_acc;
        private System.Windows.Forms.RichTextBox tb_position;
        private System.Windows.Forms.Button bt_absmove;
        private System.Windows.Forms.RichTextBox tb_dec;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_voltage;
        private System.Windows.Forms.Label lb_current;
        private System.Windows.Forms.Button uCircleButton2;
        public System.Windows.Forms.Label lb_axisname;
    }
}
