using DLPPriter.Properties;
using DLPPriter.View;
using MotionCard.控件;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace DLPPriter
{
    public partial class MainFrom : Form
    {
        public MainFrom()
        {
            InitializeComponent();
        }


        private System.Drawing.Point mouseLocation;//表示鼠标对于窗口左上角的坐标的负数
        private bool isDragging;//标识鼠标是否按下
        private System.Windows.Forms.Timer timer;

        private void MainFrom_Load(object sender, EventArgs e)
        {

            this.tabControl1.TabPages.Clear();
            this.tabControl1.TabPages.Add("打印界面");
            this.tabControl1.TabPages[0].HorizontalScroll.Enabled = this.tabControl1.TabPages[0].VerticalScroll.Enabled = true;
            AutoFrom.GetInstance.TopLevel = false;
            AutoFrom.GetInstance.Dock = DockStyle.Fill;
            this.tabControl1.TabPages[0].Controls.Add(AutoFrom.GetInstance);
            AutoFrom.GetInstance.Show();
            AutoFrom.GetInstance.Visible = true;


            this.tabControl1.TabPages.Add("调试界面");
            this.tabControl1.TabPages[1].HorizontalScroll.Enabled = this.tabControl1.TabPages[1].VerticalScroll.Enabled = true;
            MotionFrom.GetInstance.TopLevel = false;
            MotionFrom.GetInstance.Dock = DockStyle.Fill;
            this.tabControl1.TabPages[1].Controls.Add(MotionFrom.GetInstance);
            MotionFrom.GetInstance.Show();
            MotionFrom.GetInstance.Visible = true;


            this.tabControl1.TabPages.Add("光机界面");
            this.tabControl1.TabPages[2].HorizontalScroll.Enabled = this.tabControl1.TabPages[2].VerticalScroll.Enabled = true;
            ProjectorFrom.GetInstance.TopLevel = false;
            ProjectorFrom.GetInstance.Dock = DockStyle.Fill;
            this.tabControl1.TabPages[2].Controls.Add(ProjectorFrom.GetInstance);
            ProjectorFrom.GetInstance.Show();
            ProjectorFrom.GetInstance.Visible = true;

            PictureShow PL = new PictureShow();
            WorkSystem.SendImageEvent += PL.SysncImageChanged;

            Screen[] screens = Screen.AllScreens;
            if(screens.Length > 1)
            {
                Screen screen = screens[1];
                PL.StartPosition = FormStartPosition.Manual;
                PL.Location = screen.WorkingArea.Location;
                PL.Size = screen.Bounds.Size;
                PL.Show();
            }

            WorkSystem.UpdateFromMessageEvent += UpdateMessage;

            


            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += UpdateFromEvent;
            timer.Start();

            Bitmap bitmap = new Bitmap(screens[1].Bounds.Width, screens[1].Bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            WorkSystem.SendImage(null, new SyncShowBitmapEventArg() { ProductImage = bitmap });
        }

        private void UpdateFromEvent(object sender, EventArgs e)
        {
            try
            {
                this.toolStripLabel3.Text = DateTime.Now.ToString();
                this.toolStripLabel5.BackgroundImage = WorkSystem.MotionInstructionDic.Count > 0 ? Resources.绿色按钮 : Resources.红色按钮;
                this.toolStripLabel7.BackgroundImage = WorkSystem.ProjrctorDic.Count > 0 ? Resources.绿色按钮 : Resources.红色按钮;
                if(WorkSystem.ProjrctorDic.Count > 0)
                {
                    int curt = WorkSystem.CmdReadCurrent(00);
                    this.toolStripLabel9.Text = curt.ToString();
                    var states = WorkSystem.CmdGetLedStatus();
                    if (states == USBProjector.LedMotor.LED_ON)
                        this.toolStripLabel11.Text = "开灯";
                    else
                        this.toolStripLabel11.Text = "关灯";
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"主界面更新事件中发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void UpdateMessage(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    AyncShowMessageEventArg ayncShow = e as AyncShowMessageEventArg;
                    if (ayncShow != null)
                    {
                        if (this.richTextBox1.Lines.Length > 200)
                        {
                            this.richTextBox1.Clear();
                        }
                        switch (ayncShow.MLogLevel)
                        {
                            case Log.LogLevel.Info:
                                this.richTextBox1.SelectionBackColor = Color.Black;
                                this.richTextBox1.ForeColor = Color.White;
                                break;
                            case Log.LogLevel.Warn:
                                this.richTextBox1.SelectionBackColor = Color.Yellow;
                                this.richTextBox1.ForeColor = Color.White;
                                break;
                            case Log.LogLevel.Alarm:
                                this.richTextBox1.SelectionBackColor = Color.Red;
                                this.richTextBox1.ForeColor = Color.White;
                                break;
                        }
                        this.richTextBox1.AppendText($"{ayncShow.StrMsg}\r\n");
                    }
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"界面信息更新时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                }
            });
        }

        private void ChangeForm(object sender, EventArgs e)
        {
            try
            {
                UCircleButton button = (UCircleButton)sender;
                if (button != null)
                {
                    if (!button.Enabled)
                        return;
                    switch (button.Tag)
                    {
                        case "0":
                            AutoFrom.GetInstance.Visible = true;
                            MotionFrom.GetInstance.Visible = false;
                            ProjectorFrom.GetInstance.Visible = false;
                            break;
                        case "1":
                            AutoFrom.GetInstance.Visible = false;
                            MotionFrom.GetInstance.Visible = true;
                            ProjectorFrom.GetInstance.Visible = false;
                            break;
                        case "2":
                            AutoFrom.GetInstance.Visible = false;
                            MotionFrom.GetInstance.Visible = false;
                            ProjectorFrom.GetInstance.Visible = true;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"界面按钮进行按下时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void WinForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseLocation = new System.Drawing.Point(-e.X, -e.Y);
                isDragging = true;
            }
        }

        private void WinForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Drawing.Point newMouseLocation = MousePosition;
                newMouseLocation.Offset(mouseLocation.X, mouseLocation.Y);
                Location = newMouseLocation;
            }
        }

        private void WinForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
            GC.Collect();
        }
    }
}
