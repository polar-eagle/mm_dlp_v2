using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace DLPPriter.View
{
    public partial class AutoFrom : Form
    {
        public AutoFrom()
        {
            InitializeComponent();
        }

        private static AutoFrom M_AutoFrom;

        public static AutoFrom GetInstance
        {
            get
            {
                if( M_AutoFrom == null )
                    M_AutoFrom = new AutoFrom();
                return M_AutoFrom;
            }
        }


        internal void SysncImageChanged(object sender, EventArgs e )
        {
            try
            {
                SyncShowBitmapEventArg arg = e as SyncShowBitmapEventArg;
                if(arg != null && arg.ProductImage != null )
                {
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox1.Image = arg.ProductImage;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"响应图片处理时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void AutoFrom_Load(object sender, EventArgs e)
        {
            this.bt_run.Enabled = true;
            this.bt_pance.Enabled = this.bt_stop.Enabled = false;
            WorkSystem.SendImageEvent += SysncImageChanged;
            foreach (var btn in this.Controls.OfType<Button>())
            {
                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeaver;
            }
        }

        private void bt_run_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                folderBrowser.ShowDialog();
                string str = folderBrowser.SelectedPath;
                if (string.IsNullOrEmpty(str)) return;
                WorkSystem.IsManualStop = false;
                WorkSystem.IsManualPause = false;
                WorkSystem.ManualPause.Reset();
                this.bt_run.Enabled = false;
                this.bt_pance.Enabled = true;
                this.bt_stop.Enabled = false;
                WorkSystem.AsyncRunGCode(str, CallBackRun);
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"按下运行按钮时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void CallBackRun()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_run.Enabled = true;
                this.bt_pance.Enabled = false;
                this.bt_stop.Enabled = false;
                WorkSystem.IsManualStop = false;
                WorkSystem.IsManualPause = false; WorkSystem.ManualPause.Set();
            }));
        }

        private void bt_pance_Click(object sender, EventArgs e)
        {
            try
            {
                if (!WorkSystem.IsManualPause)
                {
                    WorkSystem.IsManualPause = true;
                    WorkSystem.ManualPause.Reset();
                    this.bt_stop.Enabled = true;
                }
                else
                {
                    WorkSystem.IsManualPause = false;
                    WorkSystem.ManualPause.Set();
                    this.bt_stop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"按下暂停按钮时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            try
            {
                WorkSystem.IsManualStop = true;
                WorkSystem.IsManualPause = false;
                WorkSystem.ManualPause.Set();
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"按下停止按钮时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                if (button != null)
                {
                    Color color = Color.FromArgb(button.BackColor.R + 20, button.BackColor.G + 20, button.BackColor.B + 20);
                    button.BackColor = color;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"鼠标进入按钮上方进行更改背景色时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void Button_MouseLeaver(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                if (button != null)
                {
                    Color color = Color.FromArgb(button.BackColor.R - 20, button.BackColor.G - 20, button.BackColor.B - 20);
                    button.BackColor = color;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"鼠标离开按钮上方进行更改背景色时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }

}
