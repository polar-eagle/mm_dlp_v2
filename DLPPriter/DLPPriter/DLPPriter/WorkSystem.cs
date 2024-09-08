using DLPPriter.AXIS;
using DLPPriter.ID;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using System.Runtime.InteropServices;
using System.Net.Http;

namespace DLPPriter
{
    public class WorkSystem
    {

        /// <summary>
        /// 程序启动路径(不带"\")
        /// </summary>
        public static string AppStartupPath;

        /// <summary>
        /// 控制器集合
        /// </summary>
        public static Dictionary<string, AxisController> MotionInstructionDic = new Dictionary<string, AxisController>();

        /// <summary>
        /// 轴参数集合
        /// </summary>
        public static Dictionary<Enum, MotionInfo> MotionInfoDic = new Dictionary<Enum, MotionInfo>();

        /// <summary>
        /// Com端口信息集合
        /// </summary>
        public static Dictionary<Enum, string> ComInfoDic = new Dictionary<Enum, string>();

        /// <summary>
        /// Com运行集合
        /// </summary>
        public static Dictionary<string, COM> ComRunDic = new Dictionary<string, COM>();

        /// <summary>
        /// 投影仪集合
        /// </summary>
        public static Dictionary<Enum, USBProjector> ProjrctorDic = new Dictionary<Enum, USBProjector>();


        /// <summary>
        /// 是否停止
        /// </summary>
        public static bool IsManualStop = false;
        /// <summary>
        /// 是否暂停
        /// </summary>
        public static bool IsManualPause = false;
        /// <summary>
        /// 暂停检测，用来卡线程
        /// </summary>
        public static ManualResetEvent ManualPause = new ManualResetEvent(false);

        /// <summary>
        /// 界面运动按钮回调函数
        /// </summary>
        public delegate void CallBackFunction();

        /// <summary>
        /// 使用默认的时间处理委托，定义消息发布事件
        /// </summary>
        public static event EventHandler SendImageEvent;

        /// <summary>
        /// 使用默认的事件处理委托，定义界面运行LOG发布
        /// </summary>
        public static event EventHandler UpdateFromMessageEvent;

        /// <summary>
        /// 界面轴信息更新事件
        /// </summary>
        /// <param name="motion"></param>
        public delegate void UpdateAxisStation(AxisID axisID, MotionStationinfo motion);

        /// <summary>
        /// 界面轴信息更新事件
        /// </summary>
        public static event UpdateAxisStation UpdateAxisStationEvent;


        /// <summary>
        /// 触发界面轴信息更新事件
        /// </summary>
        /// <param name="axisID"></param>
        /// <param name="motion"></param>
        public static void OnUpdateAxisStation(AxisID axisID, MotionStationinfo motion)
        {
            UpdateAxisStationEvent(axisID, motion);
        }


        public static void SendImage(object sender, EventArgs e)
        {
            SendImageEvent(sender, e);
        }

        public static void UpdateMessage(object sender, EventArgs e)
        {
            UpdateFromMessageEvent(sender, e);
        }

        public static void LoadParameters()
        {
            AddMotionInstruction();
            AddMotionInfo();
            AddComRun();
            AddProjector();
            SetAxisInitianl();
            WorkSystem.CMDLEDOFF();
        }


        public async static Task GetAxisStation()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        foreach(var item in MotionInfoDic)
                        {
                            if (MotionInstructionDic.ContainsKey(item.Value.IpAddress))
                            {
                                WorkSystem.OnUpdateAxisStation((AxisID)item.Key, WorkSystem.GetAxisStatus(item.Key));
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"后台执行获取轴坐标时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                }
            });
        }


        #region 控制器运行

        /// <summary>
        /// 添加控制器
        /// </summary>
        public static void AddMotionInstruction()
        {
            try
            {
                MotionInstructionDic.Clear();
                string str = AppStartupPath + @"\参数文件\Config.xml";
                if (!File.Exists(str)) return;
                Files.OperationXML operationXML = new Files.OperationXML(str);
                if (operationXML == null) return;
                var Counts = operationXML.ReadAttributes("MotionCardCount");
                if(Counts == null || Counts.Count == 0) return;
                int Count = int.Parse(Counts["Count"]);
                List<string> ips = new List<string>();
                var IPAdress = operationXML.ReadAttributes("IpAdress");
                if (IPAdress == null || IPAdress.Count == 0 || IPAdress.Count != Count) return;
                for(int i = 1; i <= Count; i++)
                {
                    ips.Add(IPAdress[$"Card{i}"]);
                }
                for(int i = 0; i < ips.Count; i++)
                {
                    MotionInstructionDic.Add(ips[i], new AxisController());
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"针对控制器初始化时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 添加执行器信息
        /// </summary>
        public static void AddMotionInfo()
        {
            try
            {
                MotionInfoDic.Clear();
                string str = AppStartupPath + @"\参数文件\AxisInfo.xml";
                if (!File.Exists(str)) return;
                Files.OperationXML operationXML = new Files.OperationXML(str);
                if (operationXML == null) return;
                List<List<string>> allAxisAttributes = operationXML.ReadAllAttributes("AxisInfo", new string[] { "轴ID号", "控制器IP", "丝杆导程", "执行模式", "最大限位", "最小限位", "原位坐标", "加速度", "减速度", "速度", "是否开启限位" });
                if (allAxisAttributes == null && allAxisAttributes.Count == 0) return;
                foreach(List<string> Line in allAxisAttributes)
                {
                    AxisID axisID = (AxisID)Enum.Parse(typeof(AxisID), Line[0]);
                    MotionInfo motion = new MotionInfo();
                    motion.Id = byte.Parse(Line[1]);
                    motion.IpAddress = Line[2];
                    motion.Leader = int.Parse(Line[3]);
                    motion.Mode = (Actuator.ActuatorMode)Enum.Parse(typeof(Actuator.ActuatorMode), Line[4]);
                    motion.MaximumPosition = double.Parse(Line[5]);
                    motion.MinimunPosition = double.Parse(Line[6]);
                    motion.HomingPosition = double.Parse(Line[7]);
                    motion.AccSpeed = double.Parse(Line[8]);
                    motion.DeccSpeed = double.Parse(Line[9]);
                    motion.Speed = double.Parse(Line[10]);
                    motion.IsPositionLimitEnable = bool.Parse(Line[11]);
                    MotionInfoDic.Add(axisID, motion);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"添加执行器的信息时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }



        /// <summary>
        /// 获取执行器状态
        /// </summary>
        /// <param name="axisname">自定义轴名称</param>
        /// <returns></returns>
        public static MotionStationinfo GetAxisStatus(Enum axisname)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return null;
                MotionStationinfo motion = new MotionStationinfo();
                motion.Enabled = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].isEnable(MotionInfoDic[axisname].Id);
                motion.IsOnline = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].isOnline(MotionInfoDic[axisname].Id);
                motion.Position = AxisPulseToMM(axisname, MotionInstructionDic[MotionInfoDic[axisname].IpAddress].getPosition(MotionInfoDic[axisname].Id));
                motion.Current = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].getCurrent(MotionInfoDic[axisname].Id);
                motion.Voltage = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].getVelocity(MotionInfoDic[axisname].Id);
                motion.MotorTemperature = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].getMotorTemperature(MotionInfoDic[axisname].Id);
                motion.Errorcode = MotionInstructionDic[MotionInfoDic[axisname].IpAddress].getErrorCode(MotionInfoDic[axisname].Id);
                return motion;
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"获取{axisname}状态时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 设置轴执行器的参数信息
        /// </summary>
        /// <param name="axisname">自定义轴名称</param>
        public static void SetAxisSetting(Enum axisname)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearError(MotionInfoDic[axisname].Id);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, Actuator.ActuatorMode.Mode_Homing);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearHomingInfo(MotionInfoDic[axisname].Id);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setMaximumPosition(MotionInfoDic[axisname].Id, (float)AxisMMToPulse(axisname, MotionInfoDic[axisname].MaximumPosition));
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setMinimumPosition(MotionInfoDic[axisname].Id, (float)AxisMMToPulse(axisname, MotionInfoDic[axisname].MinimunPosition));
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].enablePositionLimit(MotionInfoDic[axisname].Id, MotionInfoDic[axisname].IsPositionLimitEnable);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, MotionInfoDic[axisname].Mode);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].saveAllParams(MotionInfoDic[axisname].Id);
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}参数进行写入时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置轴执行器的参数并初始化
        /// </summary>
        public static void SetAxisInitianl()
        {
            try
            {
                if(MotionInstructionDic.Count > 0 && MotionInfoDic.Count > 0)
                {
                    foreach(var item in MotionInfoDic)
                    {
                        MotionInstructionDic[MotionInfoDic[item.Key].IpAddress].clearError(MotionInfoDic[item.Key].Id);
                        Thread.Sleep(100);
                        WorkSystem.MotorONOFF(item.Key, true);
                        Thread.Sleep(100);
                        MotionInstructionDic[MotionInfoDic[item.Key].IpAddress].activateActuatorMode(MotionInfoDic[item.Key].Id, MotionInfoDic[item.Key].Mode);
                        Thread.Sleep(100);
                        MotionInstructionDic[MotionInfoDic[item.Key].IpAddress].saveAllParams(MotionInfoDic[item.Key].Id);
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对DLP轴参数进行重新设置时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置轴运动模式
        /// </summary>
        /// <param name="axisname"></param>
        /// <param name="mode"></param>
        public static void SetAxisMode(Enum axisname, Actuator.ActuatorMode mode)
        {
            try
            {
                if (MotionInstructionDic.Count == 0) return;

                switch ((AxisID)axisname)
                {
                    case AxisID.ZUP:
                        MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, mode);
                        break;
                    case AxisID.ZDown:
                        MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, mode);
                        break;
                    case AxisID.RMotor:
                        MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, mode);
                        break;
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"设置轴运动模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置轴执行器的参数信息
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="ipadress">控制器IP地址</param>
        /// <param name="id">执行器id</param>
        /// <param name="leader">丝杆导程</param>
        /// <param name="mode">执行器mode</param>
        /// <param name="maxposition">最大限位</param>
        /// <param name="minposition">最小限位</param>
        /// <param name="homingposition">原点信息</param>
        /// <param name="ishomingenable">是否进行开启限位</param>
        public static void SetAxisSetting(Enum axisname, string ipadress, byte id, int leader, Actuator.ActuatorMode mode, double maxposition, double minposition, double homingposition, bool ishomingenable, double acc, double dec, double speed)
        {
            try
            {
                if (MotionInstructionDic.Count == 0) return;

                switch((AxisID)axisname)
                {
                    case AxisID.ZUP:
                        MotionInstructionDic[ipadress].clearError(id);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMaximumPosition(id, -(float)AxisMMToPulse(axisname, minposition));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMinimumPosition(id, -(float)AxisMMToPulse(axisname, maxposition));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].enablePositionLimit(id, ishomingenable);
                        Thread.Sleep(100);
                        AxisSetSpeed(AxisID.ZUP, acc, dec, speed);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].saveAllParams(id);
                        break;
                    case AxisID.ZDown:
                        MotionInstructionDic[ipadress].clearError(id);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMaximumPosition(id, (float)AxisMMToPulse(axisname, maxposition));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMinimumPosition(id, (float)AxisMMToPulse(axisname, minposition));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].enablePositionLimit(id, ishomingenable);
                        Thread.Sleep(100);
                        AxisSetSpeed(AxisID.ZDown, acc, dec, speed);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].saveAllParams(id);
                        break;
                    case AxisID.RMotor:
                        MotionInstructionDic[ipadress].clearError(id);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMaximumPosition(id, -(float)AxisMMToPulse(axisname, minposition * (360 / 5)));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].setMinimumPosition(id, -(float)AxisMMToPulse(axisname, maxposition * (360 / 5)));
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].enablePositionLimit(id, ishomingenable);
                        Thread.Sleep(100);
                        AxisSetSpeed(AxisID.RMotor, acc, dec, speed);
                        Thread.Sleep(100);
                        MotionInstructionDic[ipadress].saveAllParams(id);
                        break;
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}参数进行写入时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置轴零点
        /// </summary>
        /// <param name="axisname"></param>
        public static void SetHomingPosition(Enum axisname)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearError(MotionInfoDic[axisname].Id);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, Actuator.ActuatorMode.Mode_Homing);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearHomingInfo(MotionInfoDic[axisname].Id);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setHomingPosition(MotionInfoDic[axisname].Id, 0);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].enablePositionLimit(MotionInfoDic[axisname].Id, MotionInfoDic[axisname].IsPositionLimitEnable);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].activateActuatorMode(MotionInfoDic[axisname].Id, MotionInfoDic[axisname].Mode);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].saveAllParams(MotionInfoDic[axisname].Id);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}进行零点标注时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }




        /// <summary>
        /// 指定执行器进行清除零点坐标信息
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        public static void ClearHomingPosition(Enum axisname)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearHomingInfo(MotionInfoDic[axisname].Id);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].saveAllParams(MotionInfoDic[axisname].Id);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行清除零点信息时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 指定轴进行开关使能
        /// </summary>
        /// <param name="axisname">自定义名称</param>
        /// <param name="enable">使能信号</param>
        public static void MotorONOFF(Enum axisname, bool enable)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                if (enable)
                    MotionInstructionDic[MotionInfoDic[axisname].IpAddress].enableActuator(MotionInfoDic[axisname].Id);
                else
                    MotionInstructionDic[MotionInfoDic[axisname].IpAddress].disenableActuator(MotionInfoDic[axisname].Id);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}电机进行上使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }



        /// <summary>
        /// 指定执行器进行异常错误清除
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        public static void ClearErrored(Enum axisname)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].clearError(MotionInfoDic[axisname].Id);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行错误异常清除时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 毫米转脉冲值
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="mmvalue">位置值</param>
        /// <returns></returns>
        public static double AxisMMToPulse(Enum axisname, double mmvalue)
        {
            return mmvalue / MotionInfoDic[axisname].Leader;
        }

        /// <summary>
        /// 脉冲值转毫米
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="pulsevalue">位置值</param>
        /// <returns></returns>
        public static double AxisPulseToMM(Enum axisname, double pulsevalue)
        {
            return pulsevalue * MotionInfoDic[axisname].Leader;
        }

        /// <summary>
        /// 指定执行器进行加减速度及速度的设定
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">运行速度</param>
        public static void AxisMove(Enum axisname, double acc, double dec, double speed, double pos)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionAcceleration(MotionInfoDic[axisname].Id, (float)acc);
                Thread.Sleep(50);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionDeceleration(MotionInfoDic[axisname].Id, (float)dec);
                Thread.Sleep(50);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionMaxVelocity(MotionInfoDic[axisname].Id, (float)speed);
                Thread.Sleep(50);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setPosition(MotionInfoDic[axisname].Id, (float)AxisMMToPulse(axisname, pos));
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行设定速度及加减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 指定执行器进行加减速度及速度设定
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">速度</param>
        public static void AxisSetSpeed(Enum axisname, double acc, double dec, double speed)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionAcceleration(MotionInfoDic[axisname].Id, (float)acc);
                Thread.Sleep(50);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionDeceleration(MotionInfoDic[axisname].Id, (float)dec);
                Thread.Sleep(50);
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setProfilePositionMaxVelocity(MotionInfoDic[axisname].Id, (float)speed);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器设定速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 指定执行器进行绝对值定位
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="pos">绝对位置</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        public static void AxisAbsMove(Enum axisname, double pos, bool isblock = false, int timeout = -1)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                if (isblock)
                {
                    if (IsManualPause) ManualPause.WaitOne();
                    if (IsManualStop) return;
                }
                MotionInstructionDic[MotionInfoDic[axisname].IpAddress].setPosition(MotionInfoDic[axisname].Id, (float)AxisMMToPulse(axisname, pos));
                timeout = timeout < 0 ? int.MaxValue : timeout;
                int i = 0;
                while (i++ < timeout)
                {
                    //if(isblock)
                    //{
                    //    if(IsManualStop)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        return;
                    //    }
                    //    if(IsManualPause)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        ManualPause.WaitOne();
                    //        MotorONOFF(axisname, true);
                    //        AxisMove(axisname, acc, dec, speed, pos);
                    //    }
                    //}
                    MotionStationinfo motion = GetAxisStatus(axisname);
                    if (!motion.IsOnline || !motion.Enabled || MotionInstructionDic[MotionInfoDic[axisname].IpAddress].Errcode.FirstOrDefault(q => q.Value == motion.Errorcode).Key != "0000")
                    {
                        return;
                    }
                    if (Math.Abs(motion.Position - pos) < 0.1)
                        return;
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行绝对定位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 执行器异步执行绝对位移移动
        /// </summary>
        /// <param name="axisname">自定义轴名称</param>
        /// <param name="pos">移动坐标</param>
        /// <param name="callBack">自定义回调函数</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public async static Task AsyncAxisAbsMove(Enum axisname, double pos, CallBackFunction callBack, bool isblock = false, int timeout = -1)
        {
            await Task.Run(() =>
            {
                try
                {
                    AxisAbsMove(axisname, pos, isblock, timeout);
                    callBack?.Invoke();
                }
                catch (Exception ex) 
                {
                    ToolTipBox.Instance.WarnShowDialog($"执行器异步执行时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }


        /// <summary>
        /// 指定执行器进行绝对位移移动
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="pos">位置</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">速度</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        public static void AxisAbsMove(Enum axisname, double pos, double acc, double dec, double speed, bool isblock = false, int timeout = -1)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                if(isblock)
                {
                    if (IsManualPause) ManualPause.WaitOne();
                    if (IsManualStop) return;                    
                }
                AxisMove(axisname, acc, dec, speed, pos);               
                timeout = timeout < 0 ? int.MaxValue : timeout;
                int i = 0;
                while(i++ < timeout)
                {
                    //if(isblock)
                    //{
                    //    if(IsManualStop)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        return;
                    //    }
                    //    if(IsManualPause)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        ManualPause.WaitOne();
                    //        MotorONOFF(axisname, true);
                    //        AxisMove(axisname, acc, dec, speed, pos);
                    //    }
                    //}
                    MotionStationinfo motion = GetAxisStatus(axisname);
                    if(!motion.IsOnline || !motion.Enabled || MotionInstructionDic[MotionInfoDic[axisname].IpAddress].Errcode.FirstOrDefault(q => q.Value == motion.Errorcode).Key != "0000")
                    {
                        return;
                    }
                    if (Math.Abs(motion.Position - pos) < 0.1)
                        return;
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行绝对位置定位移动时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 指定执行器进行异步绝对位移移动
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="pos">位置</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">速度</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public async static Task AsyncAxisAbsMove(Enum axisname, double pos, double acc, double dec, double speed, CallBackFunction callBack, bool isblock = false, int timeout = -1)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                    if (isblock)
                    {
                        if (IsManualPause) ManualPause.WaitOne();
                        if (IsManualStop) return;
                    }
                    AxisAbsMove(axisname, pos, acc, dec, speed, isblock, timeout);
                    callBack?.Invoke();
                }
                catch(Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行异步绝对位置定位移动时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 指定执行器进行相对位置位移移动
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="relpos">相对位移距离</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">速度</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        public static void AxisRelMove(Enum axisname, double relpos, double acc, double dec, double speed, bool isblock = false, int timeout = -1)
        {
            try
            {
                if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                if (isblock)
                {
                    if (IsManualPause) ManualPause.WaitOne();
                    if (IsManualStop) return;
                }
                MotionStationinfo motion = GetAxisStatus(axisname);
                double pos = motion.Position + relpos;
                AxisMove(axisname, acc, dec, speed, pos);
                timeout = timeout < 0 ? int.MaxValue : timeout;
                int i = 0;
                while(i++ < timeout)
                {
                    //if (isblock)
                    //{
                    //    if (IsManualStop)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        return;
                    //    }
                    //    if (IsManualPause)
                    //    {
                    //        MotorONOFF(axisname, false);
                    //        ManualPause.WaitOne();
                    //        MotorONOFF(axisname, true);
                    //        AxisMove(axisname, acc, dec, speed, pos);
                    //    }
                    //}
                    motion = GetAxisStatus(axisname);
                    if (!motion.IsOnline || !motion.Enabled || MotionInstructionDic[MotionInfoDic[axisname].IpAddress].Errcode.FirstOrDefault(q => q.Value == motion.Errorcode).Key != "0000")
                    {
                        return;
                    }
                    if (Math.Abs(motion.Position - pos) < 0.1)
                        return;
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行相对移动位移时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 指定执行器进行异步相对移动位移
        /// </summary>
        /// <param name="axisname">自定义执行器名称</param>
        /// <param name="relpos">相对位移间距</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="speed">速度</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="isblock">是否检测机床停止及暂停</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public async static Task AsyncAxisRelMove(Enum axisname, double relpos, double acc, double dec, double speed, CallBackFunction callBack, bool isblock = false, int timeout = -1)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (MotionInstructionDic.Count == 0 && !MotionInfoDic.ContainsKey(axisname)) return;
                    if (isblock)
                    {
                        if (IsManualPause) ManualPause.WaitOne();
                        if (IsManualStop) return;
                    }
                    AxisRelMove(axisname, relpos, acc, dec, speed, isblock, timeout);
                    callBack?.Invoke();
                }
                catch(Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"对{axisname}执行器进行异步相对移动位移时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }


        #endregion


        #region 添加IO控制器

        /// <summary>
        /// 添加IO控制器
        /// </summary>
        public static void AddComRun()
        {
            try
            {
                ComInfoDic.Clear();
                ComRunDic.Clear();
                string str = AppStartupPath + @"\参数文件\Config.xml";
                if (!File.Exists(str)) return;
                Files.OperationXML operationXML = new Files.OperationXML(str);
                if (operationXML == null) return;
                var Counts = operationXML.ReadAttributes("IOCOM");
                if (Counts == null || Counts.Count == 0) return;
                string ComName = Counts["ComName"];
                if(string.IsNullOrEmpty(ComName)) return;
                ComInfoDic.Add(ComID.IOCard, ComName);
                ComRunDic.Add(ComName, new COM(ComName));
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"添加Io控制器时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 风扇进行开关
        /// </summary>
        /// <param name="comid">Com卡</param>
        /// <param name="isopen">打开/关闭</param>
        public static void FanOpenClose(Enum comid, bool isopen)
        {
            try
            {
                if(ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    ComRunDic[ComInfoDic[comid]].SendCom(isopen ? 'a' : 'b');
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"风扇{isopen}时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async static Task AsyncFanOpenClose(Enum comid, bool isopen, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    FanOpenClose(comid, isopen);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步风扇打开关闭时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 清洗进行开关
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="isopen">打开/关闭</param>
        public static void CleanOpenClose(Enum comid, bool isopen)
        {
            try
            {
                if(ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    ComRunDic[ComInfoDic[comid]].SendCom(isopen ? 'c' : 'd');
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"清洗{isopen}时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        public async static Task AsyncCleanOpenClose(Enum comid, bool isopen, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    CleanOpenClose(comid, isopen);
                    callBack?.Invoke();
                }
                catch(Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步清洗开关关闭时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 酒精废料流出的电磁阀开关
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="isopen">打开/关闭</param>
        public static void AlcoholSolenoidOpenClose(Enum comid, bool isopen, int count)
        {
            try
            {
                if (ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    for(int i = 0; i < count; i++)
                    {
                        ComRunDic[ComInfoDic[comid]].SendCom(isopen ? 'm' : 'n');
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"酒精废料流出的电磁阀开关时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        public async static Task AsyncAlcoholSolenoidOpenClose(Enum comid, bool isopen, int count, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    AlcoholSolenoidOpenClose(comid, isopen, count);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步酒精废料流出的电磁阀开关时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }


        /// <summary>
        /// 材料1进行送入回流
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="idfeed">送入/回流</param>
        /// <param name="count">几圈</param>
        public static void Material1(Enum comid, bool idfeed, int count)
        {
            try
            {
                if(ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    WorkSystem.AxisAbsMove(AxisID.ZUP, -160);
                    WorkSystem.AxisAbsMove(AxisID.ZDown, -50);
                    WorkSystem.AxisAbsMove(AxisID.RMotor, -(2.5 * (360 / 5)));
                    for(int i = 0; i < count; i++)
                    {
                        ComRunDic[ComInfoDic[comid]].SendCom(idfeed ? 'e' : 'f');
                    }
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"材料1进行送入回流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async static Task AsyncMaterial1(Enum comid, bool isfeed, int count, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    Material1(comid, isfeed, count);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步材料1进行送入会流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 材料2进行送入回流
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="isfeed">送入/回流</param>
        /// <param name="count">几圈</param>
        public static void Material2(Enum comid, bool isfeed, int count)
        {
            try
            {
                if (ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    WorkSystem.AxisAbsMove(AxisID.ZUP, -160);
                    WorkSystem.AxisAbsMove(AxisID.ZDown, -50);
                    WorkSystem.AxisAbsMove(AxisID.RMotor, -(3.5 * (360 / 5)));
                    for (int i = 0; i < count; i++)
                    {
                        ComRunDic[ComInfoDic[comid]].SendCom(isfeed ? 'g' : 'h');
                    }
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"材料2进行送入回流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async static Task AsyncMaterial2(Enum comid, bool isfeed, int count, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    Material2(comid, isfeed, count);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步材料2进行送入会流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 材料3进行送入回流
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="isfeed">送入/回流</param>
        /// <param name="count">几圈</param>
        public static void Material3(Enum comid, bool isfeed, int count)
        {
            try
            {
                if (ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    WorkSystem.AxisAbsMove(AxisID.ZUP, -160);
                    WorkSystem.AxisAbsMove(AxisID.ZDown, -50);
                    WorkSystem.AxisAbsMove(AxisID.RMotor, -(4.5 * (360 / 5)));
                    for (int i = 0; i < count; i++)
                    {
                        ComRunDic[ComInfoDic[comid]].SendCom(isfeed ? 'i' : 'j');
                    }
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"材料3进行送入回流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        public async static Task AsyncMaterial3(Enum comid, bool isfeed, int count, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    Material3(comid, isfeed, count);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步材料3进行送入会流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }


        /// <summary>
        /// 酒精进行送入回流
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="isfeed">送入/回流</param>
        /// <param name="count">几圈</param>
        public static void AlcoholFeed(Enum comid, bool isfeed, int count)
        {
            try
            {
                if (ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        ComRunDic[ComInfoDic[comid]].SendCom(isfeed ? 'k' : 'l');
                    }
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"酒精进行送入回流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        public async static Task AsyncAlcoholFeed(Enum comid, bool isfeed, int count, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    AlcoholFeed(comid, isfeed, count);
                    callBack?.Invoke();
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步酒精进行送入会流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }

        /// <summary>
        /// 全部回流
        /// </summary>
        /// <param name="comid"></param>
        public static void BackFlow(Enum comid)
        {
            try
            {
                if (ComInfoDic.ContainsKey(comid) && ComInfoDic.Count > 0 && ComRunDic.Count > 0) 
                    ComRunDic[ComInfoDic[comid]].SendCom('o');
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"全部回流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        #endregion


        #region 投影仪控制

        public static void AddProjector()
        {
            try
            {
                ProjrctorDic.Clear();
                string str = AppStartupPath + @"\参数文件\Config.xml";
                if (!File.Exists(str)) return;
                Files.OperationXML operationXML = new Files.OperationXML(str);
                if (operationXML == null) return;
                var Counts = operationXML.ReadAttributes("ProJector");
                if (Counts == null || Counts.Count == 0) return;
                string vendorid = Counts["VendorID"];
                string productid = Counts["ProductID"];
                if (string.IsNullOrEmpty(vendorid) || string.IsNullOrEmpty(productid)) return;
                ProjrctorDic.Add(ProjectID.Projector, new USBProjector(Int32.Parse(vendorid, System.Globalization.NumberStyles.HexNumber), Int32.Parse(productid, System.Globalization.NumberStyles.HexNumber)));
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"添加Io控制器时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取投影仪设备工作状态命令
        /// </summary>
        /// <returns></returns>
        public static USBProjector.ProJectorMode GetSystemStatus()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    return ProjrctorDic[ProjectID.Projector].GetSystemSatus();
                }
                return USBProjector.ProJectorMode.错误;
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"获取投影仪工作状态时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return USBProjector.ProJectorMode.错误;
            }
        }

        /// <summary>
        /// 往投影仪中写入电流
        /// </summary>
        /// <param name="current">电流值</param>
        /// <param name="channel">通道号，一般为00</param>
        public static void CMDWriterCurrent(int current, int channel)
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CMDWriterCurrent(current, channel);
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"往投影仪中写入电流值时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 读取投影仪电流值
        /// </summary>
        /// <param name="channel">通道号，一般为00</param>
        /// <returns></returns>
        public static int CmdReadCurrent(int channel)
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    return ProjrctorDic[ProjectID.Projector].CMDReadCurrent(channel);
                }
                return int.MaxValue;
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"读取投影仪电流值时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return int.MaxValue;
            }
        }

        /// <summary>
        /// LED开灯
        /// </summary>
        public static void CMDLEDON()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CMDLEDON();
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"LED开灯命令时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// LED关灯
        /// </summary>
        public static void CMDLEDOFF()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CMDLEDOFF();
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"LED关灯命令时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 查询LED灯开关状态
        /// </summary>
        /// <returns></returns>
        public static USBProjector.LedMotor CmdGetLedStatus()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    return ProjrctorDic[ProjectID.Projector].CmdGetLedStatus();
                }
                return USBProjector.LedMotor.LED_OFF;
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"查询LED灯开关状态时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return USBProjector.LedMotor.LED_OFF;
            }
        }

        /// <summary>
        /// 投影仪开机
        /// </summary>
        public static void CmdPowerON()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CmdPowerON();
                }
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"使投影仪进行开机时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 投影仪关机
        /// </summary>
        public static void CmdPowerOFF()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CmdPowerOFF();
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"使投影仪进行关机时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 写开机默认LED关灯模式
        /// </summary>
        public static void CmdLedDefaultOff()
        {
            try
            {
                if(ProjrctorDic.Count > 0)
                {
                    ProjrctorDic[ProjectID.Projector].CmdLedDefaultOff();
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"写投影仪开机默认为LED关灯模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 写开机默认LED开灯模式
        /// </summary>
        public static void CmdLedDefaultOn()
        {
            try
            {
                if (ProjrctorDic.Count > 0)
                    ProjrctorDic[ProjectID.Projector].CmdLedDefaultOn();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"写投影仪开机默认为LED开灯模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 读开机默认LED开灯模式
        /// </summary>
        /// <returns></returns>
        public static USBProjector.LedMotor CmdLedDefaultStatus()
        {
            try
            {
                if (ProjrctorDic.Count > 0)
                    return ProjrctorDic[ProjectID.Projector].CmdLedDefaultStatus();
                return USBProjector.LedMotor.LED_OFF;
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"读开机默认LED开灯模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return USBProjector.LedMotor.LED_OFF;
            }
        }

        /// <summary>
        /// 图像进行翻转
        /// </summary>
        /// <param name="mode"></param>
        public static void CmdImageFlip(USBProjector.ImageFlipMode mode)
        {
            try
            {
                if (ProjrctorDic.Count > 0)
                    ProjrctorDic[ProjectID.Projector].CmdImageFlip(mode);
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"图像翻转时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        #endregion


        #region 文件解析并运行

        /// <summary>
        /// 异步运行Gcode代码
        /// </summary>
        /// <param name="strname"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public async static Task AsyncRunGCode(string strname, CallBackFunction callBack)
        {
            await Task.Run(() =>
            {
                try
                {
                    RunGCode(strname);
                    callBack?.Invoke();
                }
                catch(Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"异步运行GCode代码时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    callBack?.Invoke();
                }
            });
        }


        /// <summary>
        /// 运行Gcode代码
        /// </summary>
        /// <param name="strname"></param>
        public static void RunGCode(string strname)
        {
            try
            {
                if (IsManualPause)
                    ManualPause.WaitOne();
                if (IsManualStop) return;
                if (string.IsNullOrEmpty(strname))
                {
                    UpdateFromMessageEvent(null, new AyncShowMessageEventArg() { StrMsg = $"选取的目录文件为空，跳出运行！", MLogLevel = Log.LogLevel.Warn });
                    return;
                }
                if (!File.Exists(strname + "\\run.gcode"))
                {
                    UpdateFromMessageEvent(null, new AyncShowMessageEventArg() { StrMsg = $"选取的{strname}目录下未找到run.gcode文件，跳出运行！", MLogLevel = Log.LogLevel.Warn });
                    return;
                }
                using(StreamReader sr = new StreamReader(strname + "\\run.gcode"))
                {
                    do
                    {
                        string str = sr.ReadLine();
                        var st = str.Split(' ');
                        UpdateFromMessageEvent(null, new AyncShowMessageEventArg() { StrMsg = str, MLogLevel = Log.LogLevel.Info });
                        if (IsManualPause)
                            ManualPause.WaitOne();
                        if (IsManualStop) return;
                        switch (st[0])
                        {
                            case "tank":
                                double tank = double.Parse(st[1]);
                                if(st.Length > 2)
                                {
                                    double acc = double.Parse(st[2]);
                                    double dec = double.Parse(st[3]);
                                    double speed = double.Parse(st[4]);
                                    if (dec > 0)
                                        dec = -dec;
                                    AxisSetSpeed(AxisID.RMotor, acc, dec, speed);
                                }
                                AxisAbsMove(AxisID.RMotor, -(tank * (360 / 5)), true);
                                break;
                            case "fan":
                                if (st[1] == "open")
                                    FanOpenClose(ComID.IOCard, true);
                                else if (st[1] == "close")
                                    FanOpenClose(ComID.IOCard, false);
                                break;
                            case "clean":
                                if (st[1] == "open")
                                    CleanOpenClose(ComID.IOCard, true);
                                else if (st[1] == "close")
                                    CleanOpenClose(ComID.IOCard, false);
                                break;
                            case "glass":
                                double pos = double.Parse(st[1]);
                                if(st.Length > 2)
                                {
                                    double acc = double.Parse(st[2]);
                                    double dec = double.Parse(st[3]);
                                    double speed = double.Parse(st[4]);
                                    if (dec > 0)
                                        dec = -dec;
                                    AxisSetSpeed(AxisID.ZDown, acc, dec, speed);
                                }
                                pos = Math.Max(pos, -50);
                                pos = Math.Min(pos, 0);
                                AxisAbsMove(AxisID.ZDown, pos, true);
                                break;
                            case "plate":
                                double platepos = double.Parse(st[1]);
                                if(st.Length > 2)
                                {
                                    double acc = double.Parse(st[2]);
                                    double dec = double.Parse(st[3]);
                                    double speed = double.Parse(st[4]);
                                    if (dec > 0)
                                        dec = -dec;
                                    AxisSetSpeed(AxisID.ZUP, acc, dec, speed);
                                }
                                platepos = Math.Max(platepos, 0);
                                platepos = Math.Min(platepos, 160);
                                AxisAbsMove(AxisID.ZUP, -platepos, true);
                                break;
                            case "proj":
                                string path = Path.Combine(strname, st[1]);
                                Bitmap bitmap = new Bitmap(path);
                                if (bitmap != null)
                                {
                                    SendImageEvent(null, new SyncShowBitmapEventArg() { ProductImage = bitmap });
                                    double display_yime = double.Parse(st[2]);
                                    int cur = int.Parse(st[3]);
                                    Thread.Sleep(200);
                                    CMDWriterCurrent(cur, 00);
                                    CMDLEDON();
                                    Thread.Sleep((int)(display_yime * 1000));
                                    CMDLEDOFF();
                                    Bitmap bitmap1 = new Bitmap(1920, 1080);
                                    Graphics graphics = Graphics.FromImage(bitmap1);
                                    graphics.Clear(Color.Black);
                                    SendImageEvent(null, new SyncShowBitmapEventArg() { ProductImage = bitmap1 });                                 
                                }
                                else
                                {
                                    UpdateFromMessageEvent(null, new AyncShowMessageEventArg() { StrMsg = "读取图片发生异常。图片为空，请检查是否有该图片信息", MLogLevel = Log.LogLevel.Warn });
                                    sr.Close();
                                    return;
                                }
                                break;
                            case "AMS":
                                int matrialindex = int.Parse(st[1]);
                                int matrailnumber = int.Parse(st[3]);
                                if (st[2] == "backflow")
                                {
                                    switch(matrialindex)
                                    {
                                        case 0:
                                            Material1(ComID.IOCard, false, matrailnumber);
                                            break;
                                        case 1:
                                            Material2(ComID.IOCard, false, matrailnumber);
                                            break;
                                        case 2:
                                            Material3(ComID.IOCard, false, matrailnumber);
                                            break;
                                    }
                                }
                                else if (st[2] == "feed")
                                {
                                    switch(matrialindex)
                                    {
                                        case 0:
                                            Material1(ComID.IOCard, true, matrailnumber);
                                            break;
                                        case 1:
                                            Material2(ComID.IOCard, true, matrailnumber);
                                            break;
                                        case 2:
                                            Material3(ComID.IOCard, true, matrailnumber);
                                            break;
                                    }
                                }
                                break;
                            case "ASS":
                                int assnumber = int.Parse(st[2]);
                                if (st[1] == "output")
                                {
                                    AlcoholFeed(ComID.IOCard, false, assnumber);
                                }
                                else if (st[1] == "input")
                                {
                                    AlcoholFeed(ComID.IOCard, true, assnumber);
                                }
                                break;
                            case "wait":
                                int time = (int)(double.Parse(st[1]) * 1000);
                                Thread.Sleep(time);
                                break;
                            case "capture":
                                HTTP.SingleGet1("http://172.25.112.51:8080/gopro/camera/shutter/start", 5);
                                break;
                            case "cameraMode":
                                HTTP.SingleGet1("http://172.25.112.51:8080/gopro/camera/presets/set_group?id=1001", 5);
                                Thread.Sleep(1000);
                                HTTP.SingleGet1("http://172.25.112.51:8080/gopro/camera/control/wired_usb?p=1", 5);
                                break;
                            case "z_disable":
                                MotorONOFF(AxisID.ZUP, false);
                                MotorONOFF(AxisID.ZDown, false);
                                break;
                            case "z_enable":
                                MotorONOFF(AxisID.ZUP, true);
                                MotorONOFF(AxisID.ZDown, true);
                                break;
                            case "r_enable":
                                MotorONOFF(AxisID.RMotor, true);
                                break;
                            case "r_disable":
                                MotorONOFF(AxisID.RMotor, false);
                                break;
                            case "projector_close":
                                CMDLEDOFF();
                                break;
                            case "home":
                                AxisAbsMove(AxisID.ZUP, 0, true);
                                AxisAbsMove(AxisID.ZDown, 0, true);
                                break;
                            case "r":
                                int index = int.Parse(st[1]);
                                MotorONOFF(AxisID.ZUP, true);
                                MotorONOFF(AxisID.ZDown, true);
                                AxisAbsMove(AxisID.ZUP, -160, true);
                                AxisAbsMove(AxisID.ZDown, -50, true);
                                switch(index)
                                {
                                    case 0:
                                        AxisAbsMove(AxisID.RMotor, -((int)RMtoroProjectID.液槽1 * (360 / 5)), true);
                                        break;
                                    case 1:
                                        AxisAbsMove(AxisID.RMotor, -((int)RMtoroProjectID.液漕2 * (360 / 5)), true);
                                        break;
                                    case 2:
                                        AxisAbsMove(AxisID.RMotor, -((int)RMtoroProjectID.液漕3 * (360 / 5)), true);
                                        break;
                                    case 3:
                                        AxisAbsMove(AxisID.RMotor, -((int)RMtoroProjectID.酒精 * (360 / 5)), true);
                                        break;
                                    case 4:
                                        AxisAbsMove(AxisID.RMotor, -((int)RMtoroProjectID.风干 * (360 / 5)), true);
                                        break;
                                }
                                break;
                            case "backflow":
                                BackFlow(ComID.IOCard);
                                break;
                        }
                    } while (!sr.EndOfStream);
                }
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"DLP多材料设备运行GCode文件时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        #endregion

    }


    public class SyncShowBitmapEventArg : EventArgs
    {
        /// <summary>
        /// 传入图片
        /// </summary>
        public Bitmap ProductImage { get; set; }
    }


    public class AyncShowMessageEventArg:EventArgs
    {
        /// <summary>
        /// 执行Log
        /// </summary>
        public string StrMsg { get; set; }

        /// <summary>
        /// Log类型
        /// </summary>
        public Log.LogLevel MLogLevel { get; set; }
    }


    public class HTTP
    {

        /// <summary>
        /// 使用get方法获取数据（执行完毕即释放资源）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="timeout">超时：s</param>
        /// <returns></returns>
        public async static Task<string> SingleGet(string url, int timeout)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                //string str = "{\"userid\":\"1\"}";
                //StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");
                //httpRequest.Content = stringContent;
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsStringAsync();
            }
        }


        /// <summary>
        /// 使用post方法发送Json并获取数据（执行完毕并释放资源）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="content">json字符串</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public async static Task<string> SinglePostJson(string url, string content, int timeout)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, httpContent).ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// 使用get方法获取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public async static Task<string> SingleGet1(string url, int timeout)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                HttpResponseMessage httpResponse = await httpClient.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsStringAsync();
            }
        }

    }
}
