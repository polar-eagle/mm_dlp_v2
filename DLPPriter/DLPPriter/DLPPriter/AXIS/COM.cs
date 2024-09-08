using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace DLPPriter.AXIS
{
    public class COM
    {
        private string m_comname;

        private static object m_lock = new object();

        private SerialPort M_SerialPort;


        public COM(string comname)
        {
            m_comname = comname;
            OpenCom(comname);
        }

        public void OpenCom(string comname)
        {
            try
            {
                M_SerialPort = new SerialPort(comname, 9600);
                M_SerialPort.Open();
            }
            catch(Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"开启COM端口时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="inser"></param>
        public bool SendCom(char inser)
        {
            lock (m_lock)
            {
                try
                {
                    if (M_SerialPort.IsOpen)
                    {
                        byte[] bytes = ASCIIEncoding.ASCII.GetBytes(new char[] { inser });
                        M_SerialPort.Write(bytes, 0, bytes.Length);
                        //M_SerialPort.Write(ASCIIEncoding.ASCII.GetString(new byte[] {byte.Parse(inser)}));
                        Thread.Sleep(10);
                        string str = GetComSerice();
                        if (str != string.Empty)
                        {
                            if (str.StartsWith("D"))
                                return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    ToolTipBox.Instance.WarnShowDialog($"发送串口数据时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                    return false;
                }
            }
        }


        /// <summary>
        /// 接收字符串
        /// </summary>
        /// <returns></returns>
        public string GetComSerice()
        {
            try
            {
                if(M_SerialPort.IsOpen)
                {
                    int i = 0;
                    while(i++ < 100)
                    {
                        if (!M_SerialPort.IsOpen) return string.Empty;
                        if(M_SerialPort.BytesToRead > 0)
                        {
                            byte[] bytes = new byte[M_SerialPort.BytesToRead];
                            M_SerialPort.Read(bytes, 0, bytes.Length);
                            return ASCIIEncoding.ASCII.GetString(bytes);
                        }
                        Thread.Sleep(10);
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ToolTipBox.Instance.WarnShowDialog($"接收串口数据时发生错误异常\r\n{ex.Message}\r\n{ex.StackTrace}");
                return string.Empty;
            }
        }
    }
}
