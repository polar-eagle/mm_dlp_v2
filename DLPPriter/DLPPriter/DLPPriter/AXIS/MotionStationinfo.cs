using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPPriter
{
    public class MotionStationinfo
    {

        private bool m_enabled;

        private double m_position;

        private double m_current;

        private double m_voltage;

        private double m_motortemperature;

        private bool m_isonline;

        private string m_errorcode;

        public MotionStationinfo()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled">执行器是否使能</param>
        /// <param name="position">执行器坐标位置</param>
        /// <param name="current">执行器电流</param>
        /// <param name="voltage">执行器电压</param>
        /// <param name="motortemperature">执行器温度</param>
        /// <param name="isonline">执行器是否在线</param>
        /// <param name="errorcode">执行器错误信息</param>
        public MotionStationinfo(bool enabled, double position, double current, double voltage, double motortemperature, bool isonline, string errorcode)
        {
            m_enabled = enabled;
            m_position = position;
            m_current = current;
            m_voltage = voltage;
            m_motortemperature = motortemperature;
            m_isonline = isonline;
            m_errorcode = errorcode;
        }




        /// <summary>
        /// 执行器是否使能
        /// </summary>
        public bool Enabled { get => m_enabled; internal set => m_enabled = value; }

        /// <summary>
        /// 执行器坐标位置
        /// </summary>
        public double Position { get => m_position; internal set => m_position = value; }

        /// <summary>
        /// 执行器电流
        /// </summary>
        public double Current { get => m_current; internal set => m_current = value; }

        /// <summary>
        /// 执行器电压
        /// </summary>
        public double Voltage { get => m_voltage; internal set => m_voltage = value; }

        /// <summary>
        /// 执行器温度
        /// </summary>
        public double MotorTemperature { get => m_motortemperature; internal set => m_motortemperature = value;}

        /// <summary>
        /// 执行器是否在线
        /// </summary>
        public bool IsOnline { get => m_isonline; internal set => m_isonline = value; }

        /// <summary>
        /// 执行器错误信息
        /// </summary>
        public string Errorcode { get => m_errorcode; internal set => m_errorcode = value;}
    }


    public class MotionInfo
    {
        private byte m_id;
        private string m_ipaddress;
        private int m_lead;
        private Actuator.ActuatorMode m_mode;
        private double m_maximumposition;
        private double m_minimumposition;
        private double m_homingposition;
        private double m_accspeed;
        private double m_decspeed;
        private double m_speed;
        private bool m_ispositionlimitenable;

        public MotionInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipaddree">执行器所在得控制器ip</param>
        /// <param name="led">执行器丝杆导程</param>
        /// <param name="mode">执行器执行模式</param>
        /// <param name="maximumposition">执行器最大限位</param>
        /// <param name="minimunposition">执行器最小限位</param>
        /// <param name="homeingposition">执行器原位坐标</param>
        /// <param name="ispositionlimitenable">执行器是否开启限位功能</param>
        public MotionInfo(byte id, string  ipaddree, int led, Actuator.ActuatorMode mode, double maximumposition, double minimunposition, double homeingposition, double accspeed, double decspeed, double speed, bool  ispositionlimitenable)
        {
            m_id = id;
            m_ipaddress = ipaddree;
            m_lead = led;
            m_mode = mode;
            m_maximumposition = maximumposition;
            m_minimumposition = minimunposition;
            m_homingposition = homeingposition;
            m_accspeed = accspeed;
            m_decspeed = decspeed;
            m_speed = speed;
            m_ispositionlimitenable = ispositionlimitenable;
        }

        /// <summary>
        /// 执行器id
        /// </summary>
        public byte Id { get => m_id; internal set => m_id = value; }
        /// <summary>
        /// 执行器所在得控制器ip
        /// </summary>
        public string IpAddress { get => m_ipaddress; internal set => m_ipaddress = value; }
        /// <summary>
        /// 执行器丝杆导程
        /// </summary>
        public int Leader { get => m_lead; internal set => m_lead = value; }
        /// <summary>
        /// 执行器执行模式
        /// </summary>
        public Actuator.ActuatorMode Mode {  get => m_mode; internal set => m_mode = value; }
        /// <summary>
        /// 执行器最大限位
        /// </summary>
        public double MaximumPosition { get => m_maximumposition; internal set => m_maximumposition = value;}
        /// <summary>
        /// 执行器最小限位
        /// </summary>
        public double MinimunPosition { get => m_minimumposition; internal set { m_minimumposition = value; } }
        /// <summary>
        /// 执行器原位坐标
        /// </summary>
        public double HomingPosition { get => m_homingposition; internal set => m_homingposition = value;}
        /// <summary>
        /// 执行器是否开启限位功能
        /// </summary>
        public bool IsPositionLimitEnable { get => m_ispositionlimitenable; internal set => m_ispositionlimitenable = value;}
        /// <summary>
        /// 加速度
        /// </summary>
        public double AccSpeed { get => m_accspeed; internal set => m_accspeed = value; }
        /// <summary>
        /// 减速度
        /// </summary>
        public double DeccSpeed { get => m_decspeed; internal set => m_decspeed = value; }
        /// <summary>
        /// 速度
        /// </summary>
        public double Speed { get => m_speed; internal set => m_speed = value; }
    }
}
