using ApeFree.CodePlus.Algorithm.CRC;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLPPriter
{
    public class USBProjector
    {



        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object m_lock = new object();

        private Int32 m_VendorID;

        private Int32 m_ProductID;

        private UsbDevice m_usbDevice;

        private UsbEndpointReader m_UsbEndpointReader;

        private UsbEndpointWriter m_UsbEndpointWriter;


        public USBProjector (Int32 vendorid, Int32 productid)
        {
            m_VendorID = vendorid;
            m_ProductID = productid;
            USBOpen(vendorid, productid);
        }


        public void USBOpen(Int32 vendorid, Int32 productid)
        {
            UsbDeviceFinder usbFinder = new UsbDeviceFinder(vendorid, productid);
            m_usbDevice = UsbDevice.OpenUsbDevice(usbFinder);

            if(m_usbDevice == null )
            {
                int count = 0;
                while(count < 10 && m_usbDevice == null)
                {
                    Thread.Sleep(1000);
                    m_usbDevice = UsbDevice.OpenUsbDevice(usbFinder);
                    count++;
                }
            }

            if(m_usbDevice != null)
            {
                IUsbDevice wholeUSBDevice = m_usbDevice as IUsbDevice;
                if(wholeUSBDevice != null)
                {
                    wholeUSBDevice.SetConfiguration(1);

                    wholeUSBDevice.ClaimInterface(0);
                }

                m_UsbEndpointReader = m_usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                m_UsbEndpointWriter = m_usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                
            }
        }



        public bool IsOpen()
        {
            if(m_usbDevice == null)
                return false;
            return m_usbDevice.IsOpen;
        }
        


        public void USBClose()
        {
            if(IsOpen())
            {
                IUsbDevice wholeUsbDevice = m_usbDevice as IUsbDevice;
                if(!ReferenceEquals(wholeUsbDevice, null))
                {
                    wholeUsbDevice.ReleaseInterface(0);
                }

                m_usbDevice.Close();
                m_usbDevice = null;
            }
            UsbDevice.Exit();
        }

        
        public enum ProJectorMode
        {
            开机 = 01,
            待机 = 02,
            正在开机 = 04,
            正在关机 = 08,
            错误
        }


        public enum ImageFlipMode
        {
            默认 = 01,
            X轴翻转 = 02,
            Y轴翻转 = 03,
            XY轴翻转 = 04
        }

        public enum LedMotor
        {
            LED_ON = 01,
            LED_OFF = 02
        }

        public enum PowerMotor
        {
            Power_ON = 01,
            Power_OFF = 02
        }

        public T ConvertIntToString<T>(int value)
        {
            return  (T)Enum.Parse(typeof(T), value.ToString());
        }

        public string ConvertEnumToInt<T>(string name)
        {
            return ((int)Enum.Parse(typeof(T), name)).ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// 获取设备工作状态命令
        /// </summary>
        /// <returns></returns>
        public ProJectorMode GetSystemSatus()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {

                    string str = "05 00 F6 02 70";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, 6))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            switch (rev.Substring(4, 2))
                            {
                                case "01":
                                    return ProJectorMode.开机;
                                case "02":
                                    return ProJectorMode.待机;
                                case "04":
                                    return ProJectorMode.正在开机;
                                case "08":
                                    return ProJectorMode.正在关机;
                            }
                        }
                    }
                    return ProJectorMode.错误;
                }
                return ProJectorMode.错误;
            }
        }

        /// <summary>
        /// 握手命令
        /// </summary>
        /// <returns></returns>
        public bool CMDShakeHands()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string str = "05 00 0F 00 66";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// 写电流控制命令
        /// </summary>
        /// <param name="current">电流值</param>
        /// <param name="channel">通道号，一般为00</param>
        /// <returns></returns>
        public bool CMDWriterCurrent(int current, int channel)
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string currenthex = current.ToString("x").PadLeft(4, '0');
                    string channelhex = channel.ToString("x").PadLeft(2, '0');
                    string str = $"08 00 01 {currenthex} {channelhex}";
                    var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                    Crc crc = new Crc(cmCRC16Modbus);
                    string crchex = byteToHexStr(crc.Calculate(strToToHexByte(str)), true);
                    string rev = UsbSendCommand($"{str} {crchex}");
                    if (rev != null)
                    {
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 读电流控制命令
        /// </summary>
        /// <param name="channel">通道号，一般为00</param>
        /// <returns></returns>
        public int CMDReadCurrent(int channel)
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string channelhex = channel.ToString("x").PadLeft(2, '0');
                    string str = $"07 00 01 01 {channelhex}";
                    var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                    Crc crc = new Crc(cmCRC16Modbus);
                    string crchex = byteToHexStr(crc.Calculate(strToToHexByte(str)), true);
                    string rev = UsbSendCommand($"{str} {crchex}");
                    if (rev != null)
                    {
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            return int.Parse(rev.Substring(4, rev.Length - 8), System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                }
                return int.MaxValue;
            }
        }

        /// <summary>
        /// LED开灯命令
        /// </summary>
        /// <returns></returns>
        public bool CMDLEDON()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string str = "06 00 02 01 74 06";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Led关灯命令
        /// </summary>
        /// <returns></returns>
        public bool CMDLEDOFF()
        {
            lock (m_lock)
            {
                if(IsOpen())
                {
                    string str = "06 00 02 00 F4 03";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// LED灯开关状态查询命令
        /// </summary>
        /// <returns></returns>
        public LedMotor CmdGetLedStatus()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string str = "06 00 11 00 1E 00";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            switch(rev.Substring(4, 2))
                            {
                                case "01":
                                    return LedMotor.LED_ON;
                                case "00":
                                    return LedMotor.LED_OFF;
                            }
                        }
                    }
                }
                return LedMotor.LED_OFF;
            }
        }

        /// <summary>
        /// 开机命令
        /// </summary>
        /// <returns></returns>
        public bool CmdPowerON()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string str = "06 00 03 01 F2 05";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 关机命令
        /// </summary>
        /// <returns></returns>
        public bool CmdPowerOFF()
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    string str = "06 00 03 02 F2 0F";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 写开机默认LED关灯模式
        /// </summary>
        /// <returns></returns>
        public bool CmdLedDefaultOff()
        {
            lock (m_lock)
            {
                if(IsOpen())
                {
                    string str = "07 00 04 00 01 81 3D";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 写开机默认LED开灯模式
        /// </summary>
        /// <returns></returns>
        public bool CmdLedDefaultOn()
        {
            lock(m_lock)
            {
                if(IsOpen())
                {
                    string str = "07 00 04 00 00 01 38";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 读开机默认LED开灯模式
        /// </summary>
        /// <returns></returns>
        public LedMotor CmdLedDefaultStatus()
        {
            lock(m_lock)
            {
                if(IsOpen())
                {
                    string str = "06 00 04 01 60 06";
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            switch(rev.Substring(4, 2))
                            {
                                case "00":
                                    return LedMotor.LED_ON;
                                case "01":
                                    return LedMotor.LED_OFF;
                            }
                        }
                    }
                }
                return LedMotor.LED_OFF;
            }
        }

        /// <summary>
        /// 图像翻转命令
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns></returns>
        public bool CmdImageFlip(ImageFlipMode flipMode)
        {
            lock(m_lock)
            {
                if(IsOpen())
                {
                    string str = null;
                    switch(flipMode)
                    {
                        case ImageFlipMode.默认:
                            str = "07 00 09 00 01 01 DA";
                            break;
                        case ImageFlipMode.X轴翻转:
                            str = "07 00 09 00 02 01 D0";
                            break;
                        case ImageFlipMode.Y轴翻转:
                            str = "07 00 09 00 03 81 D5";
                            break;
                        case ImageFlipMode.XY轴翻转:
                            str = "07 00 09 00 04 01 C4";
                            break;
                    }
                    string rev = UsbSendCommand(str);
                    if (rev != null)
                    {
                        var cmCRC16Modbus = new CrcModel(16, 0x8005, 0x0000, 0x0000, false, false);
                        Crc crc = new Crc(cmCRC16Modbus);
                        var result = byteToHexStr(crc.Calculate(strToToHexByte(rev.Substring(0, rev.Length - 4))), true);
                        if (result == rev.Substring(rev.Length - 4, 4))
                        {
                            if (rev.Substring(4, 6) == "41636B")
                                return true;
                        }
                    }
                }
                return false;
            }
        }




        public string UsbSendCommand(string command)
        {
            lock (m_lock)
            {
                if (IsOpen())
                {
                    int byteCount = 0;
                    ErrorCode ec = m_UsbEndpointWriter.Write(strToToHexByte(command), 3000, out byteCount);
                    if (ec != ErrorCode.None)
                        return null;
                    Thread.Sleep(50);
                    return USBRecive(command);
                }
                else
                {
                    return null;
                }
            }
        }


        public string USBRecive(string command)
        {
            int i = 0;
            while(IsOpen())
            {
                if (m_UsbEndpointReader.ReadBufferSize > 0)
                {
                    byte[] readBuffer = new byte[m_UsbEndpointReader.ReadBufferSize];
                    int byteCount = 0;
                    ErrorCode ec = m_UsbEndpointReader.Read(readBuffer, 3000, out byteCount);
                    if(ec == ErrorCode.None)
                    {
                        if (byteCount > 0)
                        {
                            return byteToHexStr(readBuffer, byteCount, true);
                        }
                        else
                        {
                            if (i > 50) return null;
                            i++;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            return null;
        }


        public string byteToHexStr(byte[] bytes, bool ishighlow)
        {
            string returnStr = "";
            if (bytes != null)
            {
                if (ishighlow)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
                else
                {
                    for (int i = bytes.Length - 1; i >= 0; i--)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
            }
            return returnStr;
        }

        public string byteToHexStr(byte[] bytes, int length, bool ishighlow)
        {
            string returnStr = "";
            if (bytes != null)
            {
                if (ishighlow)
                {
                    for (int i = 0; i < length; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
                else
                {
                    for (int i = bytes.Length - 1; i >= bytes.Length - 1 - length; i--)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
            }
            return returnStr;
        }


        public byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

    }


}
