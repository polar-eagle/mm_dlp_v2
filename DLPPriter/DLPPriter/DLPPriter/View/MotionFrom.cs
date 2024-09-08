using _3DProjector;
using DLPPriter.AXIS;
using DLPPriter.ID;
using MotionCard.控件;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using Timer = System.Windows.Forms.Timer;

namespace DLPPriter.View
{
    public partial class MotionFrom : Form
    {
        public MotionFrom()
        {
            InitializeComponent();
        }

        private static MotionFrom M_MotionFrom;

        public static MotionFrom GetInstance
        {
            get
            {
                if (M_MotionFrom == null)
                    M_MotionFrom = new MotionFrom();
                return M_MotionFrom;
            }
        }


        Timer timerUpdateUI;

        /// <summary>
        /// 送入回流名称
        /// </summary>
        private string FeedFlowName = null;
        /// <summary>
        /// 轴平台名称
        /// </summary>
        private string AxisPlateformName = null;
        /// <summary>
        /// 轴位移间距
        /// </summary>
        private double AxisMoveStep = 0;
        


        private void MotionFrom_Load(object sender, EventArgs e)
        {
            UpDateLoadAxisInfo();
            foreach (var btn in this.groupBox2.Controls.OfType<Button>())
            {
                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeaver;
            }

            foreach (var btn in this.groupBox1.Controls.OfType<Button>())
            {
                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeaver;
            }

            foreach (var btn in this.Controls.OfType<Button>())
            {
                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeaver;
            }

            WorkSystem.UpdateAxisStationEvent += UpDateMotionStatesInfo;
            WorkSystem.GetAxisStation();
        }

        /// <summary>
        /// 加载轴参数信息
        /// </summary>
        private void UpDateLoadAxisInfo()
        {
            try
            {
                this.dataGridView1.Rows.Clear();
                this.Column5.DataSource = Enum.GetNames(typeof(Actuator.ActuatorMode)).ToList();
                List<string> AxisArray = new List<string>();
                for(int i = 1; i < this.dataGridView1.ColumnCount; i++)
                {
                    AxisArray.Add(this.dataGridView1.Columns[i].HeaderText);
                }
                Files.OperationXML AxisInfo = new Files.OperationXML(WorkSystem.AppStartupPath + @"\参数文件\" + "AxisInfo.xml");
                List<List<string>> allAxisInfoAttributes = AxisInfo.ReadAllAttributes("AxisInfo", AxisArray.ToArray());
                if(allAxisInfoAttributes != null && allAxisInfoAttributes.Count > 0)
                {
                    foreach(List<string> axisline in allAxisInfoAttributes)
                    {
                        this.dataGridView1.Rows.Add(axisline.ToArray());
                    }
                }
                else
                {
                    this.dataGridView1.Rows.Add(3);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"加载轴参数时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        
        private void UpDateMotionStatesInfo(AxisID axisID, MotionStationinfo stationinfo)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (stationinfo == null) return;
                switch(axisID)
                {
                    case AxisID.ZUP:
                        if (!stationinfo.IsOnline)
                            this.label1.BackColor = Color.FromArgb(30, 30, 30);
                        else if (!stationinfo.Enabled)
                            this.label1.BackColor = Color.FromArgb(100, 100, 100);
                        else if (WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[axisID].IpAddress].Errcode.FirstOrDefault(d => d.Value == stationinfo.Errorcode).Key != "0000")
                            this.label1.BackColor = Color.FromArgb(220, 80, 80);
                        else if (stationinfo.Enabled)
                            this.label1.BackColor = Color.FromArgb(80, 220, 80);
                        this.lb_ZUPPos.Text = (-stationinfo.Position).ToString("0.0000");
                        break;
                    case AxisID.ZDown:
                        if (!stationinfo.IsOnline)
                            this.label2.BackColor = Color.FromArgb(30, 30, 30);
                        else if (!stationinfo.Enabled)
                            this.label2.BackColor = Color.FromArgb(100, 100, 100);
                        else if (WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[axisID].IpAddress].Errcode.FirstOrDefault(d => d.Value == stationinfo.Errorcode).Key != "0000")
                            this.label2.BackColor = Color.FromArgb(220, 80, 80);
                        else if (stationinfo.Enabled)
                            this.label2.BackColor = Color.FromArgb(80, 220, 80);
                        this.lb_ZDownPos.Text = (stationinfo.Position).ToString("0.0000");
                        break;
                    case AxisID.RMotor:
                        if (!stationinfo.IsOnline)
                            this.label5.BackColor = Color.FromArgb(30, 30, 30);
                        else if (!stationinfo.Enabled)
                            this.label5.BackColor = Color.FromArgb(100, 100, 100);
                        else if (WorkSystem.MotionInstructionDic[WorkSystem.MotionInfoDic[axisID].IpAddress].Errcode.FirstOrDefault(d => d.Value == stationinfo.Errorcode).Key != "0000")
                            this.label5.BackColor = Color.FromArgb(220, 80, 80);
                        else if (stationinfo.Enabled)
                            this.label5.BackColor = Color.FromArgb(80, 220, 80);
                        this.lb_TurntablePos.Text = ((-stationinfo.Position) / (360 / 5)).ToString("0.0000");
                        break;
                }
            }));
        }




        
        private void TimerUpdateUI_Tick(object sender, EventArgs e)
        {
            try
            {
                if (WorkSystem.MotionInstructionDic.Count > 0 && WorkSystem.MotionInfoDic.Count > 0)
                {
                    foreach (var item in this.Controls)
                    {
                        MotionStationControl motionStation = item as MotionStationControl;
                        if (motionStation != null && motionStation.Visible)
                        {
                            MotionStationinfo motion = WorkSystem.GetAxisStatus((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname));
                            motionStation.States = motion;
                            motionStation.UpdateAxis(motion);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"定时器事件在运行时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void MotorON(object sender)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if (motionStation != null)
                {
                    WorkSystem.MotorONOFF((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), !motionStation.States.Enabled);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对电机使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void ClearError(object sender)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if (motionStation != null)
                {
                    WorkSystem.ClearErrored((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname));
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对执行器进行清除错误时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void AxisStop(object sender)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if (motionStation != null)
                {
                    WorkSystem.MotorONOFF((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), false);
                    Thread.Sleep(200);
                    WorkSystem.MotorONOFF((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), true);
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对执行器进行停止时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void ClearZero(object sender)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if(motionStation != null)
                {
                    WorkSystem.ClearHomingPosition((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname));
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对执行器进行清除原点信息时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void AxisRelMove(object sender, double acc, double dec, double speed, double pos, WorkSystem.CallBackFunction callBack)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if (motionStation != null)
                {
                    if (motionStation.Axisname == Enum.GetName(typeof(AxisID), AxisID.RMotor))
                    {
                        if (Math.Abs( WorkSystem.GetAxisStatus(ID.AxisID.ZUP).Position - (-160)) < 0.1 && Math.Abs( WorkSystem.GetAxisStatus(ID.AxisID.ZDown).Position - (-50)) < 0.1)
                        {
                            WorkSystem.AsyncAxisRelMove((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), pos, acc, dec, speed, callBack);
                        }
                        else
                        {
                            ToolTipBox.Instance.WarnShowDialog($"若使转盘转动请先将ZUP轴和ZDown轴移动到零点！");
                        }
                    }
                    else
                    {
                        WorkSystem.AsyncAxisRelMove((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), pos, acc, dec, speed, callBack);
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对执行器进行相对运动时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private void AxisAbsMove(object sender, double acc, double dec, double speed, double pos, WorkSystem.CallBackFunction callBack)
        {
            try
            {
                MotionStationControl motionStation = sender as MotionStationControl;
                if (motionStation != null)
                {
                    if (motionStation.Axisname == Enum.GetName(typeof(AxisID), AxisID.RMotor))
                    {
                        if (Math.Abs(WorkSystem.GetAxisStatus(ID.AxisID.ZUP).Position - (-160)) < 0.1 && Math.Abs(WorkSystem.GetAxisStatus(ID.AxisID.ZDown).Position - (-50)) < 0.1)
                        {
                            WorkSystem.AsyncAxisAbsMove((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), pos, acc, dec, speed, callBack);
                        }
                        else
                        {
                            ToolTipBox.Instance.WarnShowDialog($"若使转盘转动请先将ZUP轴和ZDown轴移动到零点！");
                        }
                    }
                    else
                    {
                        WorkSystem.AsyncAxisAbsMove((AxisID)Enum.Parse(typeof(AxisID), motionStation.Axisname), pos, acc, dec, speed, callBack);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对执行器进行绝对运动时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void uCircleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string filepath = WorkSystem.AppStartupPath + @"\参数文件\AxisInfo.xml";
                Files.OperationXML operationXML = new Files.OperationXML(filepath);
                operationXML.RemoveNodeAttributes("AxisInfo");
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value != "" && row.Cells[0].Value != " ")
                    {
                        Dictionary<string, string> Attributes = new Dictionary<string, string>();
                        for (int i = 1; i < row.Cells.Count; i++)
                        {
                            string pell = "";
                            if (row.Cells[i].Value != null)
                            {
                                pell = row.Cells[i].Value.ToString();
                            }
                            Attributes.Add(this.dataGridView1.Columns[i].HeaderText, pell);
                        }
                        if (row.Cells[0].Value != null)
                        {

                            operationXML.WriteAttributes("AxisInfo", row.Cells[0].Value.ToString(), Attributes);
                        }
                        else
                        {
                            ToolTipBox.Instance.WarnShowDialog($"执行器名称可能为空或非法字符\r\n请检查！");
                            return;
                        }

                    }
                }
            }
            catch(Exception ex )
            {
                ToolTipBox.Instance.WarnShowDialog($"保存轴参数时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void uCircleButton3_Click(object sender, EventArgs e)
        {
            UpDateLoadAxisInfo();
        }

        private void uCircleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.Rows.Count == 0) return;
                foreach(DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value != "" && row.Cells[0].Value != " ")
                    {
                        AxisID axisname = (AxisID)Enum.Parse(typeof(AxisID), row.Cells[0].Value.ToString());
                        byte id = byte.Parse(row.Cells[1].Value.ToString());
                        string ipadress = row.Cells[2].Value.ToString();
                        int leader = int.Parse(row.Cells[3].Value.ToString());
                        Actuator.ActuatorMode mode = (Actuator.ActuatorMode)Enum.Parse(typeof(Actuator.ActuatorMode), row.Cells[4].Value.ToString());
                        double maxpos = double.Parse(row.Cells[5].Value.ToString());
                        double minpos = double.Parse(row.Cells[6].Value.ToString());
                        double homepos = double.Parse(row.Cells[7].Value.ToString());
                        double acc = double.Parse(row.Cells[8].Value.ToString());
                        double dec = double.Parse(row.Cells[9].Value.ToString());
                        double speed = double.Parse(row.Cells[10].Value.ToString());
                        bool ishomingenable = bool.Parse(row.Cells[11].Value.ToString());
                        WorkSystem.SetAxisSetting(axisname, ipadress, id, leader, mode, maxpos, minpos, homepos, ishomingenable, acc, dec, speed);
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"往控制器中写入参数时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void MotionFrom_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                //timerUpdateUI.Start();
            }
            else
            {
                //timerUpdateUI.Stop();
            }
        }

        private void bt_fan_Click(object sender, EventArgs e)
        {
            try
            {
                this.bt_fan.Enabled = false;
                WorkSystem.AsyncFanOpenClose(ComID.IOCard, this.bt_fan.Text == "风扇开" ? true : false, CallBackFan);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"风扇按钮按下时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                this.bt_fan.Enabled = true;
            }
        }


        private void CallBackFan()
        {
            this.BeginInvoke(new Action(() =>
            {
                if (this.bt_fan.Text == "风扇开")
                {
                    this.bt_fan.Enabled = true;
                    this.bt_fan.Text = "风扇关";
                    this.bt_fan.BackColor = Color.FromArgb(80, 220, 80);
                }
                else
                {
                    this.bt_fan.Enabled = true;
                    this.bt_fan.Text = "风扇开";
                    this.bt_fan.BackColor = Color.FromArgb(50, 50, 50);
                }
            }));
        }


        private void ChangeFeedFlowKind(string feedname)
        {
            FeedFlowName = feedname;
            this.BeginInvoke(new Action(() =>
            {
                switch(feedname)
                {
                    case "材料一":
                        this.bt_Material1.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_Material2.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material3.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_cleaningagent.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "材料二":
                        this.bt_Material1.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material2.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_Material3.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_cleaningagent.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "材料三":
                        this.bt_Material1.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material2.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material3.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_cleaningagent.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "清洗剂":
                        this.bt_Material1.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material2.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_Material3.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_cleaningagent.BackColor = Color.FromArgb(80, 220, 80);
                        break;
                }
            }));
        }


        private void ChangePlatformKind(string platformname)
        {
            AxisPlateformName = platformname;
            this.BeginInvoke(new Action(() =>
            {
                switch(platformname)
                {
                    case "生长平台":
                        this.bt_GrowthPlatform.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_GlassPlatform.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_TurntablePlatform.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "玻璃平台":
                        this.bt_GrowthPlatform.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_GlassPlatform.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_TurntablePlatform.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "转台":
                        this.bt_GrowthPlatform.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_GlassPlatform.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_TurntablePlatform.BackColor = Color.FromArgb(80, 220, 80);
                        break;
                }
            }));
        }

        private void ChangeAxisMoveStep(string movestepname)
        {
            this.BeginInvoke(new Action(() =>
            {
                switch(movestepname)
                {
                    case "0.1":
                        AxisMoveStep = 0.1;
                        this.bt_step1.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_step2.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_step3.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "1":
                        AxisMoveStep = 1;
                        this.bt_step1.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_step2.BackColor = Color.FromArgb(80, 220, 80);
                        this.bt_step3.BackColor = Color.FromArgb(50, 50, 50);
                        break;
                    case "10":
                        AxisMoveStep = 10;
                        this.bt_step1.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_step2.BackColor = Color.FromArgb(50, 50, 50);
                        this.bt_step3.BackColor = Color.FromArgb(80, 220, 80);
                        break;
                }
            }));
        }


        private void bt_clean_Click(object sender, EventArgs e)
        {
            try
            {
                this.bt_cleanout.Enabled = false;
                WorkSystem.AsyncCleanOpenClose(ComID.IOCard, this.bt_cleanout.Text == "清洗开" ? true : false, CallBackClean);
            }
            catch( Exception ex )
            {
                ToolTipBox.Instance.WarnShowDialog($"清洗开关按钮按下时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                this.bt_cleanout.Enabled = true;
            }
        }

        private void CallBackClean()
        {
            this.BeginInvoke(new Action(() =>
            {
                if(this.bt_cleanout.Text == "清洗开")
                {
                    this.bt_cleanout.Enabled = true;
                    this.bt_cleanout.Text = "清洗关";
                    this.bt_cleanout.BackColor = Color.FromArgb(80, 220, 80);
                }
                else
                {
                    this.bt_cleanout.Enabled = true;
                    this.bt_cleanout.Text = "清洗开";
                    this.bt_cleanout.BackColor = Color.FromArgb(50, 50, 50);
                }
            }));
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

        private void bt_feed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FeedFlowName)) return;
            this.bt_feed.Enabled = false;
            switch(FeedFlowName)
            {
                case "材料一":
                    WorkSystem.AsyncMaterial1(ComID.IOCard, true, 1, CallBackFeed);
                    break;
                case "材料二":
                    WorkSystem.AsyncMaterial2(ComID.IOCard, true, 1, CallBackFeed);
                    break;
                case "材料三":
                    WorkSystem.AsyncMaterial3(ComID.IOCard, true, 1, CallBackFeed);
                    break;
                case "清洗剂":
                    WorkSystem.AsyncAlcoholFeed(ComID.IOCard, true, 1, CallBackFeed);
                    break;
            }
        }


        private void CallBackFeed()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_feed.Enabled = true;
            }));
        }

        private void bt_flow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FeedFlowName)) return;
            this.bt_flow.Enabled = false;
            switch(FeedFlowName)
            {
                case "材料一":
                    WorkSystem.AsyncMaterial1(ComID.IOCard, false, 1, CallBackFlow);
                    break;
                case "材料二":
                    WorkSystem.AsyncMaterial2(ComID.IOCard, false, 1, CallBackFlow);
                    break;
                case "材料三":
                    WorkSystem.AsyncMaterial3(ComID.IOCard, false, 1, CallBackFlow);
                    break;
                case "清洗剂":
                    WorkSystem.AsyncAlcoholFeed(ComID.IOCard, false, 1, CallBackFlow);
                    break;
            }
        }


        private void CallBackFlow()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_flow.Enabled = true;
            }));
        }

        private void bt_UP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AxisPlateformName) || AxisMoveStep == 0) return;
            this.bt_UP.Enabled = false;
            switch(AxisPlateformName)
            {
                case "生长平台":
                    double Zstep = WorkSystem.GetAxisStatus(AxisID.ZUP).Position - AxisMoveStep;
                    if(Zstep > -165)
                    {
                        WorkSystem.AsyncAxisAbsMove(AxisID.ZUP, Zstep, CallBackUP);
                    }
                    else
                    {
                        CallBackUP();
                    }
                    break;
                case "玻璃平台":
                    double GlassZstep = WorkSystem.GetAxisStatus(AxisID.ZDown).Position + AxisMoveStep;
                    if(GlassZstep < 5)
                    {
                        WorkSystem.AsyncAxisAbsMove(AxisID.ZDown, GlassZstep, CallBackUP);
                    }
                    else
                    {
                        CallBackUP();
                    }
                    break;
                case "转台":
                    double SerialNumber = Math.Min(Math.Round(((-WorkSystem.GetAxisStatus(AxisID.RMotor).Position) / (360 / 5)) + 1), 4);
                    if(SerialNumber <= 5.5)
                    {
                        double pos = SerialNumber * (360 / 5);
                        WorkSystem.AxisAbsMove(AxisID.ZUP, -160);
                        WorkSystem.AxisAbsMove(AxisID.ZDown, -50);
                        WorkSystem.AsyncAxisAbsMove(AxisID.RMotor, -pos, CallBackUP);
                    }
                    else
                    {
                        CallBackUP();
                    }
                    break;
            }
        }

        private void CallBackUP()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_UP.Enabled = true;
            }));
        }

        private void bt_Down_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AxisPlateformName) || AxisMoveStep == 0) return;
            this.bt_Down.Enabled = false;
            switch(AxisPlateformName)
            {
                case "生长平台":
                    double Zstep = WorkSystem.GetAxisStatus(AxisID.ZUP).Position + AxisMoveStep;
                    if(Zstep < 5)
                    {
                        WorkSystem.AsyncAxisAbsMove(AxisID.ZUP, Zstep, CallBackDown);
                    }
                    else
                    {
                        CallBackDown();
                    }
                    break;
                case "玻璃平台":
                    double GlassZstep = WorkSystem.GetAxisStatus(AxisID.ZDown).Position - AxisMoveStep;
                    if(GlassZstep > -55)
                    {
                        WorkSystem.AsyncAxisAbsMove(AxisID.ZDown, GlassZstep, CallBackDown);
                    }
                    else
                    {
                        CallBackDown();
                    }
                    break;
                case "转台":
                    double SerialNumber = Math.Max(Math.Round(((-WorkSystem.GetAxisStatus(AxisID.RMotor).Position) / (360 / 5)) - 1), 0);
                    if(SerialNumber >= -0.1)
                    {
                        double pos = (SerialNumber * (360 / 5));
                        WorkSystem.AxisAbsMove(AxisID.ZUP, -160);
                        WorkSystem.AxisAbsMove(AxisID.ZDown, -50);
                        WorkSystem.AsyncAxisAbsMove(AxisID.RMotor, -pos, CallBackDown);
                    }
                    else
                    {
                        CallBackDown();
                    }
                    break;
            }
        }


        private void CallBackDown()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.bt_Down.Enabled = true;
            }));
        }

        private void bt_Motor_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(AxisPlateformName)) return;
                switch(AxisPlateformName)
                {
                    case "生长平台":
                        WorkSystem.MotorONOFF(AxisID.ZUP, !WorkSystem.GetAxisStatus(AxisID.ZUP).Enabled);
                        Thread.Sleep(1000);
                        if (WorkSystem.GetAxisStatus(AxisID.ZUP).Enabled)
                        {                           
                            WorkSystem.SetAxisMode(AxisID.ZUP, Actuator.ActuatorMode.Mode_Profile_Pos);                            
                        }
                        break;
                    case "玻璃平台":
                        WorkSystem.MotorONOFF(AxisID.ZDown, !WorkSystem.GetAxisStatus(AxisID.ZDown).Enabled);
                        Thread.Sleep(1000);
                        if (WorkSystem.GetAxisStatus(AxisID.ZDown).Enabled)
                        {
                            WorkSystem.SetAxisMode(AxisID.ZDown, Actuator.ActuatorMode.Mode_Profile_Pos);
                        }
                        break;
                    case "转台":
                        WorkSystem.MotorONOFF(AxisID.RMotor, !WorkSystem.GetAxisStatus(AxisID.RMotor).Enabled);
                        Thread.Sleep(1000);
                        if (WorkSystem.GetAxisStatus(AxisID.RMotor).Enabled)
                        {
                            WorkSystem.SetAxisMode(AxisID.RMotor, Actuator.ActuatorMode.Mode_Profile_Pos);
                        }
                        break;
                }
                
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"使能所有轴时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_stopmove_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    WorkSystem.MotorONOFF(AxisID.ZUP, false);
                    Thread.Sleep(50);
                    WorkSystem.MotorONOFF(AxisID.ZDown, false);
                    Thread.Sleep(50);
                    WorkSystem.MotorONOFF(AxisID.RMotor, false);
                    Thread.Sleep(200);
                    WorkSystem.MotorONOFF(AxisID.ZUP, true);
                    Thread.Sleep(50);
                    WorkSystem.MotorONOFF(AxisID.ZDown, true);
                    Thread.Sleep(50);
                    WorkSystem.MotorONOFF(AxisID.RMotor, true);
                    Thread.Sleep(500);
                    WorkSystem.SetAxisMode(AxisID.ZUP, WorkSystem.MotionInfoDic[AxisID.ZUP].Mode);
                    Thread.Sleep(500);
                    WorkSystem.SetAxisMode(AxisID.ZDown, WorkSystem.MotionInfoDic[AxisID.ZDown].Mode);
                    Thread.Sleep(500);
                    WorkSystem.SetAxisMode(AxisID.RMotor, WorkSystem.MotionInfoDic[AxisID.RMotor].Mode);

                });
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"停止移动所有轴时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_clearerror_Click(object sender, EventArgs e)
        {
            try
            {
                WorkSystem.ClearErrored(AxisID.ZUP);
                WorkSystem.ClearErrored(AxisID.ZDown);
                WorkSystem.ClearErrored(AxisID.RMotor);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"清除所有轴异常时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_SetZero_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(AxisPlateformName)) return;
                switch(AxisPlateformName)
                {
                    case "生长平台":
                        WorkSystem.SetHomingPosition(AxisID.ZUP);
                        break;
                    case "玻璃平台":
                        WorkSystem.SetHomingPosition(AxisID.ZDown);
                        break;
                    case "转台":
                        WorkSystem.SetHomingPosition(AxisID.RMotor);
                        break;
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"设置{AxisPlateformName}轴原点时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void uCircleButton4_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"设置轴上下限时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void bt_Material1_Click(object sender, EventArgs e)
        {
            ChangeFeedFlowKind(this.bt_Material1.Text);
        }

        private void bt_Material2_Click(object sender, EventArgs e)
        {
            ChangeFeedFlowKind(this.bt_Material2.Text);
        }

        private void bt_Material3_Click(object sender, EventArgs e)
        {
            ChangeFeedFlowKind(this.bt_Material3.Text);
        }

        private void bt_cleaningagent_Click(object sender, EventArgs e)
        {
            ChangeFeedFlowKind(this.bt_cleaningagent.Text);
        }

        private void bt_GrowthPlatform_Click(object sender, EventArgs e)
        {
            ChangePlatformKind(this.bt_GrowthPlatform.Text);
        }

        private void bt_GlassPlatform_Click(object sender, EventArgs e)
        {
            ChangePlatformKind(this.bt_GlassPlatform.Text);
        }

        private void bt_TurntablePlatform_Click(object sender, EventArgs e)
        {
            ChangePlatformKind(this.bt_TurntablePlatform.Text);
        }

        private void bt_step1_Click(object sender, EventArgs e)
        {
            ChangeAxisMoveStep(this.bt_step1.Text);
        }

        private void bt_step2_Click(object sender, EventArgs e)
        {
            ChangeAxisMoveStep(this.bt_step2.Text);
        }

        private void bt_step3_Click(object sender, EventArgs e)
        {
            ChangeAxisMoveStep(this.bt_step3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.Rows.Count == 0) return;
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value != "" && row.Cells[0].Value != " ")
                    {
                        AxisID axisname = (AxisID)Enum.Parse(typeof(AxisID), row.Cells[0].Value.ToString());
                        Actuator.ActuatorMode mode = (Actuator.ActuatorMode)Enum.Parse(typeof(Actuator.ActuatorMode), row.Cells[4].Value.ToString());
                        WorkSystem.SetAxisMode(axisname, mode);
                    }
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"往控制器中写入运动模式参数时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
