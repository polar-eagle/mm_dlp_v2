using _3DProjector;
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
    public partial class ProjectorFrom : Form
    {
        public ProjectorFrom()
        {
            InitializeComponent();
        }

        private static ProjectorFrom M_Projectorfrom;

        public static ProjectorFrom GetInstance
        {
            get
            {
                if (M_Projectorfrom == null)
                    M_Projectorfrom = new ProjectorFrom();
                return M_Projectorfrom;
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                this.tb_currt.Text = this.trackBar1.Value.ToString();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"滑动滑块时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void ProjectorFrom_Load(object sender, EventArgs e)
        {
            try
            {
                int value = WorkSystem.CmdReadCurrent(00);
                this.tb_currt.Text = value.ToString();
                this.check_x.Checked = true;
                this.check_y.Checked = this.check_xy.Checked = false;
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"加载投影仪窗体时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_poweron_Click(object sender, EventArgs e)
        {
            try
            {
                WorkSystem.CmdPowerON();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"投影仪电源打开时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_poweroff_Click(object sender, EventArgs e)
        {
            try
            {
                WorkSystem.CmdPowerOFF();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"投影仪电源关闭时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_ledonoff_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.bt_ledonoff.Text == "开")
                {
                    WorkSystem.CMDLEDON();
                    this.bt_ledonoff.Text = "关";
                }
                else
                {
                    WorkSystem.CMDLEDOFF();
                    this.bt_ledonoff.Text = "开";
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"投影仪LED灯开关时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                this.bt_ledonoff.Text = "开";
            }
        }

        private void tb_currt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tb_currt.Text)) return;
                this.trackBar1.Value = int.Parse(this.tb_currt.Text);
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"电流值填写框的值填写时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_writercurrt_Click(object sender, EventArgs e)
        {
            try
            {
                WorkSystem.CMDWriterCurrent(this.trackBar1.Value, 00);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"修改投影仪电流值时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_readercurrt_Click(object sender, EventArgs e)
        {
            try
            {
                this.tb_currt.Text = WorkSystem.CmdReadCurrent(00).ToString();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"读取投影仪电流值时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void check_x_CheckedChanged(object sender, EventArgs e)
        {
            this.check_x.Checked = true;
            this.check_y.Checked = this.check_xy.Checked = false;
        }

        private void check_y_CheckedChanged(object sender, EventArgs e)
        {
            this.check_y.Checked = true;
            this.check_x.Checked = this.check_xy.Checked = false;
        }

        private void check_xy_CheckedChanged(object sender, EventArgs e)
        {
            this.check_xy.Checked = true;
            this.check_x.Checked = this.check_y.Checked = false;
        }

        private void bt_xyfilter_Click(object sender, EventArgs e)
        {
            try
            {
                USBProjector.ImageFlipMode flipMode = USBProjector.ImageFlipMode.默认;
                if (this.check_x.Checked)
                    flipMode = USBProjector.ImageFlipMode.X轴翻转;
                else if (this.check_y.Checked)
                    flipMode = USBProjector.ImageFlipMode.Y轴翻转;
                else if (this.check_xy.Checked)
                    flipMode = USBProjector.ImageFlipMode.XY轴翻转;
                WorkSystem.CmdImageFlip(flipMode);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"投影仪进行图像方向调换时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_load_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "JPEG文件(*.jpg)|*.jpg|GIF文件(*.gif)|*.gif|BMP文件(*.bmp)|*.bmp";
                openFileDialog.ShowDialog();
                string strname = openFileDialog.FileName;
                if (string.IsNullOrEmpty(strname)) return;
                Bitmap bitmap = new Bitmap(strname);
                WorkSystem.SendImage(this, new SyncShowBitmapEventArg() { ProductImage = bitmap });
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"加载图像时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
