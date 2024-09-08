using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace DLPPriter.AXIS
{
    public class MotionInstruction
    {
        private List<byte> m_UnifiedID = new List<byte>();

        public Dictionary<uint, string> Errcode = new Dictionary<uint, string>();

        private string m_ipaddress;
        /// <summary>
        /// 连接控制器是否成功
        /// </summary>
        private bool IsInitial = false;

        public MotionInstruction (string ipaddress)
        {
            m_ipaddress = ipaddress;
            LoadErrcode();
            if(ConnectController(ipaddress))
            {
                this.IsInitial = true; 
            }
            else
            {
                this.IsInitial= false;
            }
        }

        /// <summary>
        /// 连接控制器
        /// </summary>
        /// <param name="ipaddress">控制器IP</param>
        /// <returns></returns>
        private bool ConnectController(string ipaddress)
        {
            try
            {
                m_UnifiedID = Actuator.getUnifiedIDGroup(ipaddress);
                if(m_UnifiedID.Count == 0)
                {
                    ToolTipBox.Instance.WarnShowDialog($"在{ipaddress}控制器中未发现可以执行得执行器！");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                ToolTipBox.Instance.WarnShowDialog($"连接控制器失败\r\n{e.Message}\r\n{e.StackTrace}");
                return false;
            }
        }

        public void LoadErrcode()
        {
            Errcode.Add(0, "控制器运行正常！");
            Errcode.Add(1, "执行器过牙错误！");
            Errcode.Add(2, "执行器欠压错误！");
            Errcode.Add(4, "执行器堵转错误！");
            Errcode.Add(8, "执行器过温错误！");
            Errcode.Add(16, "执行器读写错误！");
            Errcode.Add(32, "执行器多圈计数错误！");
            Errcode.Add(64, "执行器逆变器温度器错误！");
            Errcode.Add(128, "执行器CAN通讯错误！");
            Errcode.Add(256, "执行器温度传感器错误！");
            Errcode.Add(512, "阶跃过大！");
            Errcode.Add(1024, "执行器DRV保护！");
            Errcode.Add(2048, "编码器失效！");
            Errcode.Add(2049, "执行器未连接错误！");
            Errcode.Add(2050, "CAN通信转换版未连接错误！");
            Errcode.Add(2051, "无可用ip地址错误！");
            Errcode.Add(2052, "执行器非正常关机错误！");
            Errcode.Add(2053, "执行器关机时参数保存错误！");
            Errcode.Add(2054, "通讯端口已绑定！");
            Errcode.Add(2055, "执行器ID不唯一错误！");
            Errcode.Add(2056, "通讯ip地址冲突！");
            Errcode.Add(65535, "未知错误！");
        }

        /// <summary>
        /// 使能指定执行器
        /// </summary>
        /// <param name="id"></param>
        public void MotorON(byte id)
        {
            try
            {
                if(this.IsInitial)
                {
                    if(m_UnifiedID.Contains(id))
                    {
                        Actuator.enableActuator(id, m_ipaddress);
                    }
                }
            }
            catch(Exception ex)
            {
                Tools.Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器使能时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 失能指定执行器
        /// </summary>
        /// <param name="id"></param>
        public void MotorOFF(byte id)
        {
            try
            {
                if(this.IsInitial)
                {
                    if (m_UnifiedID.Contains(id))
                        Actuator.disableActuator(id, m_ipaddress);
                }
            }
            catch (Exception ex)
            {
                Tools.Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器关闭使能时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Warn);
            }
        }

        /// <summary>
        /// 激活执行器得指定模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="mode">执行模式</param>
        public void MotorActuatorMode(byte id, Actuator.ActuatorMode mode)
        {
            try
            {
                if(this.IsInitial)
                {
                    if(m_UnifiedID.Contains(id))
                        Actuator.activateActuatorMode(id, mode, m_ipaddress);
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器激活指定模式时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="pos">目标位置，单位圈数</param>
        public void SetPosition(byte id, double pos)
        {
            try
            {
                if(this.IsInitial)
                {
                    if ((m_UnifiedID.Contains(id)))
                        Actuator.setPosition(id, pos, m_ipaddress);
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定位置时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取执行器当前位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetPosition(byte id)
        {
            try
            {
                if(this.IsInitial)
                {
                    if ((m_UnifiedID.Contains(id)))
                        return Actuator.getPosition(id, true, m_ipaddress);
                }
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取位置时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 设置位置环得最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="max">位置环得最大限位</param>
        public void SetMaximumPosition(byte id, double max)
        {
            try
            {
                if (this.IsInitial)
                {
                    if ((m_UnifiedID.Contains(id)))
                        Actuator.setMaximumPosition(id, max, m_ipaddress);
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定位置环最大限位时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取位置环得最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetMaximumPosition(byte id)
        {
            try
            {
                if(this.IsInitial)
                {
                    if ((m_UnifiedID.Contains(id)))
                        return Actuator.getMaximumPosition(id, true, m_ipaddress);
                }
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器读取位置环最大限位时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 设置位置环最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="min">位置环最小限位</param>
        public void SetMinmumPosition(byte id, double min)
        {
            try
            {
                if(this.IsInitial)
                {
                    if ((m_UnifiedID.Contains(id)))
                        Actuator.setMinimumPosition(id, min, m_ipaddress);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定位置环最小限位时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取位置环最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetMinimumPosition(byte id)
        {
            try
            {
                if (this.IsInitial)
                {
                    if (m_UnifiedID.Contains(id))
                        Actuator.getMinimumPosition(id, true, m_ipaddress);
                }
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器读取位置环最小限位时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 使能/失能执行器限位功能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enabled">使能/失能</param>
        public void EnablePositionLimit(byte id, bool enabled)
        {
            try
            {
                if(this.IsInitial)
                {
                    if((m_UnifiedID.Contains(id)))
                        Actuator.enablePositionLimit(id, enabled, m_ipaddress);
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器使能/失能执行器限位功能时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 读取执行器限位功能失能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool IsPositionLimitEnable(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.isPositionLimitEnable(id, true, m_ipaddress);
                return false;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器读取执行器限位功能是否打开时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return false;
            }
        }

        /// <summary>
        /// 设定执行器得零位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="homingPosition">执行器得零位</param>
        public void SetHomingPosition(byte id, double homingPosition)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setHomingPosition(id, homingPosition, m_ipaddress);
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定执行器零位时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 清除homing信息，包括左右极限和零位
        /// </summary>
        /// <param name="id">执行器id</param>
        public void ClearHomingPosition(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.clearHomingInfo(id, m_ipaddress);
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器清除Homing信息时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 设置profile position模式下的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration">加速度</param>
        public void SetProfilePositionAcceleration(byte id, double acceleration)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setProfilePositionAcceleration(id, acceleration, m_ipaddress);
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定Profile position模式下的加速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetProfilePositionAcceleration(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getProfilePositionAcceleration(id, true, m_ipaddress);
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取加速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }


        /// <summary>
        /// 设置profile position模式下的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">加速度</param>
        public void SetProfilePositionDeceleration(byte id, double deceleration)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setProfilePositionDeceleration(id, deceleration, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定Profile position模式下的减速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetProfilePositionDeceleration(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getProfilePositionDeceleration(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取减速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 设置profile position模式下的最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="MaxVelocity">最大速度</param>
        public void SetProfilePositionMaxVelocity(byte id, double MaxVelocity)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setProfilePositionMaxVelocity(id, MaxVelocity, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定Profile position模式下的最大速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetProfilePositionMaxVelocity(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getProfilePositionMaxVelocity(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取最大速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="vel">速度</param>
        public void SetVelocity(byte id, double vel)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setVelocity(id, vel, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetVelocity(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getVelocity(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }


        /// <summary>
        /// 设置速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="Limit">速度限制</param>
        public void SetVelocityLimit(byte id, double Limit)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setVelocityLimit(id, Limit, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定速度限制时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetVelocityLimit(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getVelocityLimit(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取速度限制时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 设置Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration">Profile velocity模式的加速度</param>
        public void SetProfileVelocityAcceleration(byte id, double acceleration)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setProfileVelocityAcceleration(id, acceleration, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定Profile velocity模式的加速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetProfileVelocityAcceleration(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getProfileVelocityAcceleration(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取Profile velocity模式的加速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }


        /// <summary>
        /// 设置Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">Profile velocity模式的减速度</param>
        public void SetProfileVelocityDeceleration(byte id, double deceleration)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setProfileVelocityDeceleration(id, deceleration, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定Profile velocity模式的减速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetProfileVelocityDeceleration(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getProfileVelocityDeceleration(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取Profile velocity模式的减速度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }


        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="current">目标电流，单位是A</param>
        public void SetCurrent(byte id, double current)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setCurrent(id, current, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定电流时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetCurrent(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getCurrent(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取电流时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }


        /// <summary>
        /// 设置执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="current">执行器电流限制，单位是A</param>
        public void SetCurrentLimit(byte id, double limit)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.setCurrentLimit(id, limit, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器设定执行器电流限制时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetCurrentLimit(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.getCurrentLimit(id, true, m_ipaddress);
                return double.NaN;
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取执行器电流限制时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 执行器保存当前所有参数
        /// </summary>
        /// <param name="id">执行器id</param>
        public void SaveAllParams(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.saveAllParams(id, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器保存当前所有参数时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
            }
        }

        /// <summary>
        /// 获取执行器电压
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetVoltage(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.getVoltage(id, true, m_ipaddress);
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取电压时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 获取电机温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public double GetMotorTemperature(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.getMotorTemperature(id, true, m_ipaddress);
                return double.NaN;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取执行器温度时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return double.NaN;
            }
        }

        /// <summary>
        /// 获取执行器是否在线
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool IsOnLine(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains((byte)id))
                    return Actuator.isOnline(id, m_ipaddress);
                return false;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器是否在线时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return false;
            }
        }

        /// <summary>
        /// 执行器是否已经使能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool IsEnable(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    return Actuator.isEnable(id, m_ipaddress);
                return false;
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器是否已经使能时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}", Log.LogLevel.Alarm);
                return false;
            }
        }

        /// <summary>
        /// 获取执行器错误代码
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public string GetErrorcode(byte id)
        {
            try
            {
                return Errcode[Actuator.getErrorCode(id, m_ipaddress)];
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器获取错误代码时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return "未知错误！";
            }
        }

        /// <summary>
        /// 执行器掉线重连
        /// </summary>
        /// <param name="id">执行器id</param>
        public void Reconnect(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.reconnect(id, m_ipaddress);
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器掉线重连时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 执行器错误清除
        /// </summary>
        /// <param name="id">执行器id</param>
        public void ClearError(byte id)
        {
            try
            {
                if (this.IsInitial && m_UnifiedID.Contains(id))
                    Actuator.clearError(id, m_ipaddress);
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{m_ipaddress}控制器下{id}执行器清除错误时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
