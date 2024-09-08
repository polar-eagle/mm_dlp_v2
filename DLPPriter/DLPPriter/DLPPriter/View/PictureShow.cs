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
    public partial class PictureShow : Form
    {
        public PictureShow()
        {
            InitializeComponent();
        }

        internal void SysncImageChanged(object sender, EventArgs e)
        {
            try
            {
                SyncShowBitmapEventArg arg = e as SyncShowBitmapEventArg;
                if(arg != null && arg.ProductImage != null)
                {
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox1.Image = arg.ProductImage;
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"响应图片处理时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

    }
}
