using MotionCard.通讯;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using static DLPPriter.Actuator;

namespace DLPPriter.AXIS
{
    public class AxisController
    {

        private IPAddress M_IpAddress;

        private List<byte> m_UnifiedID = new List<byte>();

        public Dictionary<string, string> Errcode = new Dictionary<string, string>();

        /// <summary>
        /// 连接控制器是否成功
        /// </summary>
        private bool IsInitial = false;

        private static object M_lock = new object();

        private IPEndPoint m_IPEndPoint;

        private Socket m_Socket;


        public AxisController()
        {
            LoadErrcode();
            if(ConnectController())
            {
                this.IsInitial = true;
            }
            else
            {
                this.IsInitial = false;
            }
        }




        public void LoadErrcode()
        {
            Errcode.Add("0000", "控制器运行正常！");
            Errcode.Add("0001", "执行器过牙错误！");
            Errcode.Add("0002", "执行器欠压错误！");
            Errcode.Add("0004", "执行器堵转错误！");
            Errcode.Add("0008", "执行器过温错误！");
            Errcode.Add("0010", "执行器读写错误！");
            Errcode.Add("0020", "执行器多圈计数错误！");
            Errcode.Add("0040", "执行器逆变器温度器错误！");
            Errcode.Add("0080", "执行器CAN通讯错误！");
            Errcode.Add("0100", "执行器温度传感器错误！");
            Errcode.Add("0200", "阶跃过大！");
            Errcode.Add("0400", "执行器DRV保护！");
            Errcode.Add("0800", "编码器失效！");
            Errcode.Add("0801", "执行器未连接错误！");
            Errcode.Add("0802", "CAN通信转换版未连接错误！");
            Errcode.Add("0803", "无可用ip地址错误！");
            Errcode.Add("0804", "执行器非正常关机错误！");
            Errcode.Add("0805", "执行器关机时参数保存错误！");
            Errcode.Add("0806", "通讯端口已绑定！");
            Errcode.Add("0807", "执行器ID不唯一错误！");
            Errcode.Add("0808", "通讯ip地址冲突！");
            Errcode.Add("FFFF", "未知错误！");
        }

        /// <summary>
        /// 初始化控制器
        /// </summary>
        /// <returns></returns>
        private bool ConnectController()
        {
            try
            {
                IPEndPoint iP = new IPEndPoint(IPAddress.Parse("192.168.1.255"), 2000);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                string str = "EE 00 44 00 00 ED";
                byte[] bytes = Helperfunctions.GetIntence.strToToHexByte(str);
                socket.SendTo(bytes, bytes.Length, SocketFlags.None, iP);
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)sender;
                Thread.Sleep(500);
                while (socket.Available != 0)
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead = socket.Available;
                    int recv = socket.ReceiveFrom(buffer, ref Remote);
                    string str2 = byteToHexStr(buffer, recv);
                    if(str2.StartsWith("EE") && str2.EndsWith("ED"))
                    {
                        IPEndPoint iPEndPoint = (IPEndPoint)Remote;
                        this.M_IpAddress = iPEndPoint.Address;
                        break;
                    }
                    else
                    {
                        socket.SendTo(bytes, bytes.Length, SocketFlags.None, iP);
                    }
                }
                this.m_IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.255"), 2000);
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                string st = "EE 00 02 00 00 ED";
                byte[] bytes1 = Helperfunctions.GetIntence.strToToHexByte(st);
                m_Socket.SendTo(bytes1, bytes1.Length, SocketFlags.None, m_IPEndPoint);
                Thread.Sleep(500);
                while (m_Socket.Available != 0)
                {
                    byte[] buffer = new byte[2048];
                    int recv = m_Socket.Receive(buffer, SocketFlags.None);
                    string str2 = byteToHexStr(buffer, recv);
                    if(str2.StartsWith("EE") && str2.EndsWith("ED"))
                    {
                        if(Helperfunctions.GetIntence.byteToHexStr( Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte( str2.Substring(10,int.Parse(str2.Substring(6,4))*2)),false),false) == str2.Substring(str2.Length - 6, 4))
                        {
                            m_UnifiedID.Add(byte.Parse(str2.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                        }
                    }
                    else
                    {
                        m_Socket.SendTo(bytes1, bytes1.Length, SocketFlags.None, m_IPEndPoint);
                    }
                }
                this.IsInitial = true;
                return true;
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"连接控制器时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 使能执行器
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool enableActuator(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if(!m_UnifiedID.Contains(id))
                {
                    return false;
                }
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 2A 00 01 01 7E 80 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if(recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                Tools.Log.Instance.WriteLog($"对执行器{id}使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 失能执行器
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool disenableActuator(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id))
                {
                    return false;
                }
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 2A 00 01 00 BF 40 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "00")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tools.Log.Instance.WriteLog($"对执行器{id}使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 激活单个执行器的指定模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="mode">指定的激活模式</param>
        public void activateActuatorMode(byte id, ActuatorMode mode)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    string modehex = ((int)mode).ToString("X2");
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(modehex), false), false);
                    string str = $"EE {idhex} 07 00 01 {modehex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if(recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"激活{id}执行器的指定模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置执行器位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="pos">目标位置，单位为圈数R</param>
        public void setPosition(byte id, float pos)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 position = (Int32)(pos * Math.Pow(2, 24));
                    string poshex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(poshex), false), false);
                    string str = $"EE {idhex} 0A 00 04 {poshex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置位置时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 执行器获取当前位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getPosition(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0f;
                if (!m_UnifiedID.Contains(id)) return 0f;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 06 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string pos = StrToOrby(recive.Substring(10, 8));
                                int position = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(pos), 0);
                                float result = (float)(position / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                    return 0f;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取位置时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0f;
            }
        }

        /// <summary>
        /// 设置位置环的最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="maxpos">位置环的最大限位</param>
        public void setMaximumPosition(byte id, float maxpos)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 position = (Int32)(maxpos * Math.Pow(2, 24));
                    string poshex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(poshex), false), false);
                    string str = $"EE {idhex} 83 00 04 {poshex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置位置环最大限位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取位置环的最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getMaximumPosition(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 85 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string acchex = StrToOrby(recive.Substring(10, 8));
                                Int32 acc = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(acchex), 0);
                                float result = (float)(acc / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取位置环最大限位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 设置位置环的最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="minpos">位置环的最小限位</param>
        public void setMinimumPosition(byte id, float minpos)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 position = (Int32)(minpos * Math.Pow(2, 24));
                    string poshex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(poshex), false), false);
                    string str = $"EE {idhex} 84 00 04 {poshex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置位置环最小限位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取位置环的最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getMinimumPosition(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 86 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string acchex = StrToOrby(recive.Substring(10, 8));
                                Int32 acc = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(acchex), 0);
                                float result = (float)(acc / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取位置环最小限位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 使能/失能执行器限位功能，失能后速度模式和电流模式将不受限位影响
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        public void enablePositionLimit(byte id, bool enable)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = enable ? $"EE {idhex} 8C 00 01 01 7E 80 ED" : $"EE {idhex} 8C 00 01 00 BF 40 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {                               
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器{enable}时限位功能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 读取执行器限位功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isPositionLimitEnable(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 8B 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if(recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取限位功能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }


        /// <summary>
        /// 设置执行器零位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="homingPosition">执行器的零位</param>
        public void setHomingPosition(byte id, float homingPosition)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 position = (Int32)(homingPosition * Math.Pow(2, 24));
                    string poshex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(poshex), false), false);
                    string str = $"EE {idhex} 87 00 04 {poshex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置零位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 使能/失能位置环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        public void enablePositionFilter(byte id, bool enable)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = enable ? $"EE {idhex} 78 00 01 01 7E 80 ED" : $"EE {idhex} 78 00 01 00 BF 40 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while( m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器使能/失能位置环滤波功能出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 读取执行器位置环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isPositionFilterEnable(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 79 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器读取位置环滤波功能使能/失能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 获取位置环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getPositionCutoffFrequency(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 7B 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取位置环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 设置位置环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">位置环低通滤波频率</param>
        public void setPositionCutoffFrequency(byte id, float frequency)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(frequency * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 7A 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置位置环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 清除homing信息，包括左右限位和零位
        /// </summary>
        /// <param name="id">执行器id</param>
        public void clearHomingInfo(byte id)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 88 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器清除homing信息，包括左右极限和零位时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置Profile position模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration">Profile position模式的加速度</param>
        public void setProfilePositionAcceleration(byte id, float acceleration)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 acc = (Int32)(acceleration * 17476.26);
                    string accelerationhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(acc), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(accelerationhex), false), false);
                    string str = $"EE {idhex} 20 00 04 {accelerationhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置Profile position模式的加速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 获取Profile position模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getProfilePositionAcceleration(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 1D 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while(m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string acchex = StrToOrby(recive.Substring(10, 8));
                                Int32 acc = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(acchex), 0);
                                float result = (float)(acc / 17476.26);
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取Profile position模式的加速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置Profile position模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">Profile position模式的减速度</param>
        public void setProfilePositionDeceleration(byte id, float deceleration)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 dec = (Int32)(deceleration * 17476.26);
                    string decelerationhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(dec), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(decelerationhex), false), false);
                    string str = $"EE {idhex} 21 00 04 {decelerationhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置Profile position模式的减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取Profile position模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getProfilePositionDeceleration(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 1E 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string dechex = StrToOrby(recive.Substring(10, 8));
                                Int32 dec = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(dechex), 0);
                                float result = (float)(dec / 17476.26);
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取Profile position模式的减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0f;
            }
        }

        /// <summary>
        /// 设置Profile position模式的最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="maxVelocity">Profile position模式的最大速度</param>
        public void setProfilePositionMaxVelocity(byte id, float maxVelocity)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 max = (Int32)(maxVelocity * 17476.26);
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(max), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 1F 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置Profile position模式的最大速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取Profile position模式的最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getProfilePositionMaxVelocity(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 1C 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / 17476.26);
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取Profile position模式的最大速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="vel">速度</param>
        public void setVelocity(byte id, float vel)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 max = (Int32)((vel) * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(max), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 09 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取当前速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getVelocity(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 05 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result * 6000;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 使能/失能速度环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        public void enableVelocityFilter(byte id, bool enable)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = enable ? $"EE {idhex} 74 00 01 01 7E 80 ED" : $"EE {idhex} 74 00 01 00 BF 40 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器使能/失能速度环滤波功能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 读取执速度环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isVelocityFilterEnable(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 75 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器读取速度环滤波功能使能/失能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 获取速度环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getVelocityCutoffFrequency(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 77 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取速度环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置速度环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">速度环低通滤波频率</param>
        public void setVelocityCutoffFrequency(byte id, float frequency)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(frequency * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 76 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置速度环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 设置执行器速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="limit">执行器速度限制，单位是RPM,该值不会超过速度量程</param>
        public void setVelocityLimit(byte id, float limit)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 max = (Int32)(limit * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(max), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 5A 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置速度限制时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取执行器速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getVelocityLimit(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 5B 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取速度限制时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration">Profile velocity模式的加速度</param>
        public void setProfileVelocityAcceleration(byte id, float acceleration)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock(M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 acc = (Int32)(acceleration * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(acc), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 26 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置Profile velocity模式的加速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getProfileVelocityAcceleration(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 23 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取Profile velocity模式的加速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">Profile velocity模式的减速度</param>
        public void setProfileVelocityDeceleration(byte id, float deceleration)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 acc = (Int32)(deceleration * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(acc), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 27 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置Profile velocity模式的减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 获取Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getProfileVelocityDeceleration(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 24 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取Profile velocity模式的减速度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="current">目标电流，单位是A</param>
        public void setCurrent(byte id, float current)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 acc = (Int32)((current / 33) * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(acc), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 08 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置电流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取当前电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getCurrent(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 04 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result * 33;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取当前电流时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 使能/失能电流环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        public void enableCurrentFilter(byte id, bool enable)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = enable ? $"EE {idhex} 70 00 01 01 7E 80 ED" : $"EE {idhex} 70 00 01 00 BF 40 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器使能/失能电流环滤波功能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 读取执电流环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isCurrentFilterEnable(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 71 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器读取电流环滤波功能失能/使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }


        /// <summary>
        /// 获取电流环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getCurrentCutoffFrequency(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 73 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电流环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 设置电流环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">电流环低通滤波频率</param>
        public void setCurrentCutoffFrequency(byte id, float frequency)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(frequency * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 72 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置电流环低通滤波频率时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 设置执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="limit">执行器电流限制,单位是A，该值不会超过电流量程</param>
        public void setCurrentLimit(byte id, float  limit)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 max = (Int32)(limit * Math.Pow(2, 24));
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(max), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 58 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置电流限制时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 获取执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getCurrentLimit(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 59 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 24));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电流限制时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 执行器保存当前所有参数,如果修改参数以后没有保存，失能后将丢弃参数修改
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool saveAllParams(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 0D 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器保存当前所有参数时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }


        /// <summary>
        /// 获取执行器电压
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getVoltage(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 45 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 4));
                                Int16 max = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / Math.Pow(2, 10));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电压时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 获取执行器堵转能量
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getLockEnergy(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 7F 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string maxhex = StrToOrby(recive.Substring(10, 8));
                                Int32 max = BitConverter.ToInt32(Helperfunctions.GetIntence.strToToHexByte(maxhex), 0);
                                float result = (float)(max / 75.225f);
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取堵转能量时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置执行器堵转能量
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="energy">执行器堵转能量，单位J</param>
        public void setLockEnergy(byte id, float energy)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int32 max = (Int32)(energy * 75.225f);
                    string maxVelocityhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(max), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(maxVelocityhex), false), false);
                    string str = $"EE {idhex} 7E 00 04 {maxVelocityhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置堵转能量时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取电机温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getMotorTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 5F 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电机温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }


        /// <summary>
        /// 获取逆变器温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getInverterTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 60 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取逆变器温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 获取电机保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getMotorProtectedTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 6C 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电机保护温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置电机保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">电机保护温度</param>
        public void setMotorProtectedTemperature(byte id, float temp)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(temp * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 6B 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置电机保护温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取电机恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getMotorRecoveryTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 6E 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取电机恢复温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置电机恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">电机恢复温度</param>
        public void setMotorRecoveryTemperature(byte id, float temp)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(temp * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 6D 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置电机恢复温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        /// <summary>
        /// 获取逆变器保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getInverterProtectedTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 62 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取逆变器保护温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置逆变器保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">逆变器保护温度</param>
        public void setInverterProtectedTemperature(byte id, float temp)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(temp * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 61 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置逆变器保护温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 获取逆变器恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public float getInverterRecoveryTemperature(byte id)
        {
            try
            {
                if (!this.IsInitial) return 0;
                if (!m_UnifiedID.Contains(id)) return 0;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 64 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            if (Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(recive.Substring(10, int.Parse(recive.Substring(6, 4)) * 2)), false), false) == recive.Substring(recive.Length - 6, 4))
                            {
                                string value = StrToOrby(recive.Substring(10, 4));
                                Int16 a = BitConverter.ToInt16(Helperfunctions.GetIntence.strToToHexByte(value), 0);
                                float result = (float)(a / Math.Pow(2, 8));
                                return result;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置逆变器恢复温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 设置逆变器恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">逆变器恢复温度</param>
        public void setInverterRecoveryTemperature(byte id, float temp)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    Int16 position = (Int16)(temp * Math.Pow(2, 8));
                    string frequencyhex = Helperfunctions.GetIntence.byteToHexStr(BitConverter.GetBytes(position), false);
                    string crc = Helperfunctions.GetIntence.byteToHexStr(Helperfunctions.GetIntence.Crc18(Helperfunctions.GetIntence.strToToHexByte(frequencyhex), false), false);
                    string str = $"EE {idhex} 63 00 02 {frequencyhex} {crc} ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器设置逆变器恢复温度时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 执行器是否在线
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isOnline(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 00 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器查找是否在线时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 执行器是否已经使能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public bool isEnable(byte id)
        {
            try
            {
                if (!this.IsInitial) return false;
                if (!m_UnifiedID.Contains(id)) return false;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 2B 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器检测是否使能时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 获取执行器当前模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public ActuatorMode getActuatorMode(byte id)
        {
            try
            {
                if (!this.IsInitial) return ActuatorMode.Mode_None;
                if (!m_UnifiedID.Contains(id)) return ActuatorMode.Mode_None;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} 2B 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                switch(recive.Substring(10,2))
                                {
                                    case "01":
                                        return ActuatorMode.Mode_Cur;
                                    case "02":
                                        return ActuatorMode.Mode_Vel;                                       
                                    case "03":
                                        return ActuatorMode.Mode_Pos;
                                    case "06":
                                        return ActuatorMode.Mode_Profile_Pos;
                                    case "07":
                                        return ActuatorMode.Mode_Profile_Vel;
                                    case "08":
                                        return ActuatorMode.Mode_Homing;
                                }
                            }
                        }
                        else
                        {
                            return ActuatorMode.Mode_None;
                        }
                    }
                    return ActuatorMode.Mode_None;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取执行器当前模式时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return ActuatorMode.Mode_None;
            }
        }

        /// <summary>
        /// 获取执行器错误代码
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <returns></returns>
        public string getErrorCode(byte id)
        {
            try
            {
                if (!this.IsInitial) return "未连接";
                if (!m_UnifiedID.Contains(id)) return "控制器中未发现该电机";
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} FF 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                return Errcode[recive.Substring(10, 4)];
                            }
                        }
                        else
                        {
                            return "通讯错误";
                        }
                    }
                    return Errcode["0000"];
                }
            }
            catch(Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器获取错误状态时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return "异常错误";
            }
        }

        /// <summary>
        /// 执行器错误清除
        /// </summary>
        /// <param name="id">执行器id</param>
        public void clearError(byte id)
        {
            try
            {
                if (!this.IsInitial) return;
                if (!m_UnifiedID.Contains(id)) return;
                lock (M_lock)
                {
                    string idhex = id.ToString("X2");
                    string str = $"EE {idhex} FE 00 00 ED";
                    byte[] send = Helperfunctions.GetIntence.strToToHexByte(str);
                    m_Socket.SendTo(send, send.Length, SocketFlags.None, m_IPEndPoint);
                    Thread.Sleep(10);
                    while (m_Socket.Available != 0)
                    {
                        byte[] buffer = new byte[2048];
                        int recv = m_Socket.Receive(buffer, SocketFlags.None);
                        string recive = byteToHexStr(buffer, recv);
                        if (recive.StartsWith("EE") && recive.EndsWith("ED"))
                        {
                            {
                                if (recive.Substring(10, 2) == "01")
                                    return;
                                else
                                    return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.WriteLog($"{id}执行器清除错误时出现错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }


        private  string byteToHexStr(byte[] bytes, int length)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        private string StrToOrby(string str)
        {
            string returnStr = "";
            if(str != null)
            {
                for(int i = str.Length - 2; i >= 0; i -= 2)
                {
                    returnStr += str.Substring(i, 2);
                }
            }
            return returnStr;
        }
    }
}
