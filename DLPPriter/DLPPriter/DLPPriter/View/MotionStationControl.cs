using DLPPriter.ID;
using MotionCard.控件;
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
    public partial class MotionStationControl : UserControl
    {
        public MotionStationControl()
        {
            InitializeComponent();
        }

        private MotionStationinfo stationinfo;

        public MotionStationinfo States
        { 
            get { return stationinfo; }
            set { stationinfo = value; }
        }

        /// <summary>
        /// 执行器自定义名称
        /// </summary>
        public string Axisname { get; set; }

        private void MotionStationControl_Load(object sender, EventArgs e)
        {
            this.lb_axisname.Text = Axisname;


            foreach (var btn in this.panel1.Controls.OfType<UCircleButton>())
            {
                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeaver;
            }

            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Tick += UpdateUI;
            //timer.Start();
        }

        private void UpdateUI(object sender, EventArgs e)
        {
            if(stationinfo != null)
            {
                if (Axisname == Enum.GetName(typeof(AxisID), AxisID.ZUP))
                {
                    this.lb_pos.Text = (-States.Position).ToString("0.0000");
                }
                else
                {
                    this.lb_pos.Text = States.Position.ToString("0.0000");
                }
                this.lb_current.Text = States.Current.ToString("0.00") + "A";
                this.lb_voltage.Text = States.Voltage.ToString("0.00") + "V";
                this.lb_axisname.BackColor = States.IsOnline ? Color.FromArgb(50, 200, 50) : Color.FromArgb(50, 50, 50);
                this.uCircleButton1.BackColor = States.Enabled ? Color.FromArgb(50, 200, 50) : Color.FromArgb(50, 50, 50);
                this.bt_clearerror.BackColor = WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[(AxisID)Enum.Parse(typeof(AxisID), Axisname)].IpAddress].Errcode.FirstOrDefault(d => d.Value == States.Errorcode).Key == "0000" ? Color.FromArgb(50, 50, 50) : Color.FromArgb(200, 50, 50);                
            }
        }


        public void UpdateAxis(MotionStationinfo motion)
        {
            this.BeginInvoke(new Action(() =>
            {
                if(motion == null) return;
                if (Axisname == Enum.GetName(typeof(AxisID), AxisID.ZUP))
                {
                    this.lb_pos.Text = (-motion.Position).ToString("0.0000");
                }
                else
                {
                    this.lb_pos.Text = motion.Position.ToString("0.0000");
                }
                this.lb_current.Text = motion.Current.ToString("0.00") + "A";
                this.lb_voltage.Text = motion.Voltage.ToString("0.00") + "V";
                this.lb_axisname.BackColor = motion.IsOnline ? Color.FromArgb(50, 200, 50) : Color.FromArgb(50, 50, 50);
                this.uCircleButton1.BackColor = motion.Enabled ? Color.FromArgb(50, 200, 50) : Color.FromArgb(50, 50, 50);
                this.bt_clearerror.BackColor = WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[(AxisID)Enum.Parse(typeof(AxisID), Axisname)].IpAddress].Errcode.FirstOrDefault(d => d.Value == motion.Errorcode).Key == "0000" ? Color.FromArgb(50, 50, 50) : Color.FromArgb(200, 50, 50);
            }));
        }

        /// <summary>
        /// 点击按钮后触发运动及复位等操作
        /// </summary>
        /// <param name="sender"></param>
        public delegate void ButtonEventHander(object sender);

        /// <summary>
        /// 运动委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="speed"></param>
        /// <param name="pos"></param>
        public delegate void MotionMoveEventHander(object sender, double acc, double dec, double speed, double pos, WorkSystem.CallBackFunction callBack);

        /// <summary>
        /// 轴上下使能事件
        /// </summary>
        public event ButtonEventHander MotorEventHander;

        /// <summary>
        /// 轴清除异常事件
        /// </summary>
        public event ButtonEventHander ClearErrorEventHander;

        /// <summary>
        /// 停止运动事件
        /// </summary>
        public event ButtonEventHander StopMoveEventHander;

        /// <summary>
        /// 清除原点信息事件
        /// </summary>
        public event ButtonEventHander ClearZeroEventHander;

        /// <summary>
        /// 相对运动事件
        /// </summary>
        public event MotionMoveEventHander RelMoveEventHander;

        /// <summary>
        /// 绝对运动事件
        /// </summary>
        public event MotionMoveEventHander AbsMoveEventHander;

        private void bt_motor_Click(object sender, EventArgs e)
        {
            if(WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[(AxisID)Enum.Parse(typeof(AxisID), Axisname)].IpAddress].Errcode.FirstOrDefault(d => d.Value == States.Errorcode).Key == "0000")
            {
                MotorEventHander(this);
            }
        }

        private void bt_clearerror_Click(object sender, EventArgs e)
        {
            ClearErrorEventHander(this);
        }

        private void bt_stopmove_Click(object sender, EventArgs e)
        {
            StopMoveEventHander(this);
        }

        private void bt_relmove_Click(object sender, EventArgs e)
        {
            if(WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[(AxisID)Enum.Parse(typeof(AxisID), Axisname)].IpAddress].Errcode.FirstOrDefault(d => d.Value == States.Errorcode).Key == "0000" && !string.IsNullOrEmpty(this.tb_acc.Text) && !string.IsNullOrEmpty(this.tb_dec.Text) && !string.IsNullOrEmpty(this.tb_speed.Text) && !string.IsNullOrEmpty(this.tb_position.Text))
            {
                RelMoveEventHander(this, double.Parse(this.tb_acc.Text), double.Parse(this.tb_dec.Text), double.Parse(this.tb_speed.Text), double.Parse(this.tb_position.Text), RelMoveCallBack);
            }
        }

        private void RelMoveCallBack()
        {
            this.BeginInvoke(new Action(() =>
            {
            }));
        }

        private void bt_absmove_Click(object sender, EventArgs e)
        {
            if (WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[(AxisID)Enum.Parse(typeof(AxisID), Axisname)].IpAddress].Errcode.FirstOrDefault(d => d.Value == States.Errorcode).Key == "0000" && !string.IsNullOrEmpty(this.tb_acc.Text) && !string.IsNullOrEmpty(this.tb_dec.Text) && !string.IsNullOrEmpty(this.tb_speed.Text) && !string.IsNullOrEmpty(this.tb_position.Text))
            {
                this.bt_absmove.Enabled = false;
                switch(Axisname)
                {
                    case "ZUP":
                        AbsMoveEventHander(this, double.Parse(this.tb_acc.Text), double.Parse(this.tb_dec.Text), double.Parse(this.tb_speed.Text), double.Parse(this.tb_position.Text), AbsMoveCallBack);
                        break;
                    case "ZDown":
                        AbsMoveEventHander(this, double.Parse(this.tb_acc.Text), double.Parse(this.tb_dec.Text), double.Parse(this.tb_speed.Text), double.Parse(this.tb_position.Text), AbsMoveCallBack);
                        break;
                    case "RMotor":
                        AbsMoveEventHander(this, double.Parse(this.tb_acc.Text), double.Parse(this.tb_dec.Text), double.Parse(this.tb_speed.Text), double.Parse(this.tb_position.Text), AbsMoveCallBack);
                        break;
                }
            }
        }

        private void AbsMoveCallBack()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_absmove.Enabled = true;
            }));
        }

        private void bt_clearZero_Click(object sender, EventArgs e)
        {
            ClearZeroEventHander(this);
        }

        private void lb_axisname_MouseEnter(object sender, EventArgs e)
        {

        }


        private void Button_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                UCircleButton button = (UCircleButton)sender;
                if(button != null)
                {
                    Color color = Color.FromArgb(button.BgColor.R + 20, button.BgColor.G + 20, button.BgColor.B + 20);
                    button.BgColor = button.BgColor2 = color;
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"鼠标进入按钮上方进行更改背景色时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void Button_MouseLeaver(object sender, EventArgs e)
        {
            try
            {
                UCircleButton button = (UCircleButton)sender;
                if (button != null)
                {
                    Color color = Color.FromArgb(button.BgColor.R - 20, button.BgColor.G - 20, button.BgColor.B - 20);
                    button.BgColor = button.BgColor2 = color;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"鼠标离开按钮上方进行更改背景色时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void uCircleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if(ToolTipBox.Instance.WarnShowDialog($"是否对{Axisname}进行设定零点！",1))
                {
                    WorkSystem.SetHomingPosition((AxisID)Enum.Parse(typeof(AxisID), Axisname));
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"{Axisname}在设定原点时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void tb_position_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if(!string .IsNullOrEmpty(this.tb_position.Text))
                {
                    float vaule = 0;
                    if(float.TryParse(this.tb_position.Text, out vaule))
                    {
                        vaule = float.Parse(this.tb_position.Text);
                        switch(Axisname)
                        {
                            case "ZUP":
                                if(vaule < 0)
                                {
                                    this.tb_position.Text = (-vaule).ToString();
                                }
                                break;
                            case "ZDown":
                                if(vaule > 0)
                                {
                                    this.tb_position.Text = (-vaule).ToString();
                                }
                                break;
                            case "RMotor":
                                if(vaule > 0)
                                {
                                    this.tb_position.Text = (-vaule).ToString();
                                }
                                break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"{Axisname}在填入坐标时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void tb_acc_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.tb_acc.Text))
                {
                    float vaule = 0;
                    if (float.TryParse(this.tb_acc.Text, out vaule))
                    {
                        vaule = float.Parse(this.tb_acc.Text);
                        if(vaule < 0)
                        {
                            this.tb_acc.Text = (-vaule).ToString();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"{Axisname}在填入加速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void tb_dec_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.tb_dec.Text))
                {
                    float vaule = 0;
                    if (float.TryParse(this.tb_dec.Text, out vaule))
                    {
                        vaule = float.Parse(this.tb_dec.Text);
                        if (vaule > 0)
                        {
                            this.tb_dec.Text = (-vaule).ToString();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"{Axisname}在填入减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void tb_speed_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.tb_speed.Text))
                {
                    float vaule = 0;
                    if (float.TryParse(this.tb_speed.Text, out vaule))
                    {
                        vaule = float.Parse(this.tb_speed.Text);
                        if (vaule < 0)
                        {
                            this.tb_speed.Text = (-vaule).ToString();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"{Axisname}在填入速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
