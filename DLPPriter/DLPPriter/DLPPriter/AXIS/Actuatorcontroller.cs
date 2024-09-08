using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DLPPriter
{
    public class Actuator
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct UnifiedID
        {
            public byte actuatorID;
            public string ipAddress;
        }

        public enum ErrorsDefine
        {
            ERR_NONE = 0,
            ///执行器过压错误
            ERR_ACTUATOR_OVERVOLTAGE = 0x01,
            ///执行器欠压错误
            ERR_ACTUATOR_UNDERVOLTAGE = 0x02,
            ///执行器堵转错误
            ERR_ACTUATOR_LOCKED_ROTOR = 0x04,
            ///执行器过温错误
            ERR_ACTUATOR_OVERHEATING = 0x08,
            ///执行器读写错误
            ERR_ACTUATOR_READ_OR_WRITE = 0x10,
            ///执行器多圈计数错误
            ERR_ACTUATOR_MULTI_TURN = 0x20,
            ///执行器逆变器温度器错误
            ERR_INVERTOR_TEMPERATURE_SENSOR = 0x40,
            ///执行器CAN通信错误
            ERR_CAN_COMMUNICATION = 0x80,
            ///执行器温度传感器错误
            ERR_ACTUATOR_TEMPERATURE_SENSOR = 0x100,
            ///阶跃过大
            ERR_STEP_OVER = 0x200,
            ///执行器DRV保护
            ERR_DRV_PROTECTION = 0x400,
            ///编码器失效
            ERR_CODER_DISABLED = 0x800,
            ///执行器未连接错误
            ERR_ACTUATOR_DISCONNECTION = 0x801,
            ///CAN通信转换板未连接错误
            ERR_CAN_DISCONNECTION = 0x802,
            ///无可用ip地址错误
            ERR_IP_ADDRESS_NOT_FOUND = 0x803,
            ///执行器非正常关机错误
            ERR_ABNORMAL_SHUTDOWN = 0x804,
            ///执行器关机时参数保存错误
            ERR_SHUTDOWN_SAVING = 0x805,
            ///通信端口已绑定
            ERR_IP_HAS_BIND = 0x806,
            ///通信ip地址冲突
            ERR_IP_CONFLICT = 0x808,
            ///执行器ID不唯一错误
            ERR_ID_UNUNIQUE = 0x807,
            ERR_UNKOWN = 0xffff
        };

        public enum ActuatorMode
        {
            Mode_None,
            ///电流模式
            Mode_Cur = 1,
            ///速度模式
            Mode_Vel = 2,
            ///位置模式
            Mode_Pos = 3,
            ///暂未实现
            Mode_Teaching = 4,
            ///profile位置模式，比较于位置模式，该模式有加速减速过程
            Mode_Profile_Pos = 6,
            ///profile速度模式，比较于速度模式，该模式有加速减速过程
            Mode_Profile_Vel = 7,
            ///归零模式
            Mode_Homing = 8,
        };

        /// <summary>
        /// 查找所有已连接的执行器
        /// </summary>
        /// <param name="ec">错误代码，如果找不到可用执行器，返回对应错误代码</param>
        /// <returns>返回所有查找到的执行器的UnifiedID</returns>
        [DllImport("actuatorControllerd.dll")]        
        public static extern List<UnifiedID> lookupActuators(ref ErrorsDefine ec);

        /// <summary>
        /// 处理控制器事件
        /// </summary>
        [DllImport("actuatorControllerd.dll")]
        public static extern void processEvents();

        /// <summary>
        /// 是否有可用执行器
        /// </summary>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool hasAvailableActuator();

        /// <summary>
        /// 获取所有执行器Id数组
        /// </summary>
        /// <returns>UnifiedID数组</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern List<byte> getActuatorIdArray();

        /// <summary>
        /// 获取所有执行器UnifiedID数组
        /// </summary>
        /// <returns>UnifiedID数组</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern List<UnifiedID> getActuatorUnifiedIDArray();

        /// <summary>
        /// 获取指定ip地址下所有执行器的UnifiedID
        /// </summary>
        /// <param name="ipAddress">ip地址的字符串</param>
        /// <returns>返回同一通信单元内的所有执行器ID</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern List<byte> getUnifiedIDGroup( string ipAddress);

        /// <summary>
        /// 使能所有执行器
        /// </summary>
        /// <returns>全部使能成功返回true，否则返回false</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool enableAllActuators();

        /// <summary>
        /// 失能所有执行器
        /// </summary>
        /// <returns>全部失能成功返回true，否则返回false</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool disableAllActuators();

        /// <summary>
        /// 使能指定执行器
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool enableActuator(byte id, string ipAddress);

        /// <summary>
        /// 使能指定执行器
        /// </summary>
        /// <param name="unifiedIDs">执行器UnifiedID数组</param>
        /// <returns>执行器使能成功返回true,否则返回false</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool enableActuatorInBatch(ref List<UnifiedID> unifiedIDs);


        /// <summary>
        /// 失能指定执行器
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipaddress">目标ip地址字符串</param>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool disableActuator(byte id, string ipaddress);

        /// <summary>
        /// 激活单个执行器的指定模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="nMode">指定的激活模式</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void activateActuatorMode(byte id, ActuatorMode nMode, string ipAddress);


        /// <summary>
        /// 激活执行器的指定模式
        /// </summary>
        /// <param name="idArray">执行器的id数组</param>
        /// <param name="nMode">要激活的模式</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void activateActuatorModeInBantch(List<byte> idArray, ActuatorMode nMode);


        /// <summary>
        /// 激活执行器的指定模式
        /// </summary>
        /// <param name="UnifuedIDArray">执行器UnifiedID数组</param>
        /// <param name="nMode">要激活的模式</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void activateActuatorModeInBantch(List<UnifiedID> UnifuedIDArray, ActuatorMode nMode);

        /// <summary>
        /// 开启或关闭执行器自动刷新功能，自动请求执行器电流、速度、位置、电压、温度、逆变器温度（默认关闭此功能）
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bOpen">是否开启</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void switchAutoRefresh(byte id, bool bOpen, string ipAddress);

        /// <summary>
        /// 设置自动刷新时间间隔（默认时间间隔为1s）
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="mSec">毫秒数</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setAutoRefreshInterval(byte id, uint mSec, string ipAddress);


        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="pos"><目标位置，单位是圈数/param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPosition(byte id, double pos, string ipAddress);


        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一此位置读取，并等待返回，如果为false，则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前位置，单位是转数</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPosition(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的比例
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="kp">位置环的比例</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionKp(byte id, double kp, string ipAddress);

        /// <summary>
        /// 获取位置环的比例
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次位置环的比例，并等待返回，如果为false，则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="iAddress">目标ip地址字符串</param>
        /// <returns>位置环的比例</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionKp(byte id, bool bRefresh, string iAddress);

        /// <summary>
        /// 设置位置环的积分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="Ki">位置环的积分</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionKi(byte id, double Ki, string ipAddress);

        /// <summary>
        /// 获取位置环的积分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次位置环的积分,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的积分</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionKi(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的微分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="kd">位置环的微分</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionKd(byte id, double kd, string ipAddress);

        /// <summary>
        /// 获取位置环的微分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次位置环的微分,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的微分</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionKd(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的最大输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="max">位置环的最大输出限幅,有效值范围为(0,1]</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionUmax(byte id, double max, string ipAddress);

        /// <summary>
        /// 获取位置环的最大输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次最大输出限幅,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的最大输出限幅</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionUmax(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的最小输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="min">位置环的最小输出限幅,有效值范围为[-1,0)</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionUmin(byte id, double min, string ipAddress);

        /// <summary>
        /// 获取位置环的最小输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次最大输出限幅,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的最小输出限幅</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionUmin(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的限位偏移
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="offset">位置环的限位偏移</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void SetPositionOffset(byte id, double offset, string ipAddress);

        /// <summary>
        /// 获取位置环的限位偏移
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次限位偏移,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的限位偏移</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionOffset(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="maxPos">位置环的最大限位</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setMaximumPosition(byte id, double maxPos, string ipAddress);

        /// <summary>
        /// 获取位置环的最大限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次最大限位,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的最大限位</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getMaximumPosition(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环的最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="minPos">位置环的最小限位</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setMinimumPosition(byte id, double minPos, string ipAddress);

        /// <summary>
        /// 获取位置环的最小限位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次最小限位,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环的最小限位</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getMinimumPosition(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 使能/失能执行器限位功能，失能后速度模式和电流模式将不受限位影响
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void enablePositionLimit(byte id, bool enable, string ipAddress);

        /// <summary>
        /// 读取执行器限位功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次限位功能使能/失能,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isPositionLimitEnable(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置执行器的零位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="homingPos">执行器的零位</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setHomingPosition(byte id, double homingPos, string ipAddress);

        /// <summary>
        /// 使能/失能位置环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void enablePositionFilter(byte id, bool enable, string ipAddress);

        /// <summary>
        /// 读取执位置环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次位置环滤波功能使能/失能,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isPositionFilterEnable(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取位置环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次低通滤波频率,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>位置环低通滤波频率</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getPositionCutoffFrequency(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置位置环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">位置环低通滤波频率</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setPositionCutoffFrequency(byte id, double frequency, string ipAddress);

        /// <summary>
        /// 清除homing信息，包括左右极限和零位
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void clearHomingInfo(byte id, string ipAddress);

        /// <summary>
        /// 设置Profile position模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration">Profile position模式的加速度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setProfilePositionAcceleration(byte id, double acceleration, string ipAddress);

        /// <summary>
        /// 获取Profile position模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次Profile position模式的加速度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>Profile position模式的加速度</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getProfilePositionAcceleration(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置Profile position模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">Profile position模式的减速度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setProfilePositionDeceleration(byte id, double deceleration, string ipAddress);

        /// <summary>
        /// 获取Profile position模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次Profile position模式的减速度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>Profile position模式的减速度</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getProfilePositionDeceleration(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置Profile position模式的最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="maxVelocity">Profile position模式的最大速度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setProfilePositionMaxVelocity(byte id, double maxVelocity, string ipAddress);

        /// <summary>
        /// 获取Profile position模式的最大速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次Profile position模式的最大速度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>Profile position模式的最大速度</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getProfilePositionMaxVelocity(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="vel">目标速度，单位是转/每分钟</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocity(byte id, double vel, string ipAddress);

        /// <summary>
        /// 获取当前速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新,如果为true，会自动请求一次速度读取,并等待返回，如果为false,则会立即返回最近一次请求速度返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前速度，单位是转/每分钟</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocity(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置速度环比例
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="kp">速度环比例</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityKp(byte id, double kp, string ipAddress);

        /// <summary>
        /// 获取速度环比例
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次速度环比例读取,并等待返回，如果为false,则会立即返回最近一次请求速度环比例返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前速度环比例</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityKp(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置速度环积分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ki">积分</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityKi(byte id, double ki, string ipAddress);

        /// <summary>
        /// 获取速度环积分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="brefresh">是否需要刷新，如果为true，会自动请求一次速度环积分读取,并等待返回，如果为false,则会立即返回最近一次请求速度环积分返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前速度环积分</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityKi(byte id, bool brefresh, string ipAddress);

        /// <summary>
        /// 设置速度环最大输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="max">最大输出限幅,有效值范围为（0,1]</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityUmax(byte id, double max, string ipAddress);

        /// <summary>
        /// 获取速度环最大输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次速度环最大输出限幅读取,并等待返回，如果为false,则会立即返回最近一次请求速度环最大输出限幅返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>最大输出限幅</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityUmax(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置速度环最小输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="min">最小输出限幅,有效值范围为[-1,0)</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityUmin(byte id, double min, string ipAddress);

        /// <summary>
        /// 获取速度环最小输出限幅
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次速度环最小输出限幅读取,并等待返回，如果为false,则会立即返回最近一次请求速度环最小输出限幅返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>最小输出限幅</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityUmin(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取执行器速度量程，单位RPM
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器速度量程,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器速度量程</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityRange(byte  id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 使能/失能速度环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void enableVelocityFilter(byte id, bool enable, string ipAddress);

        /// <summary>
        /// 读取执速度环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次速度环滤波功能使能/失能,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>速度环滤波功能使能/失能</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isVelocityFilterEnable(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取速度环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次低通滤波频率,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>速度环低通滤波频率</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityCutoffFrequency(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置速度环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">速度环低通滤波频率</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityCutoffFrequency(byte id, double frequency, string ipAddress);

        /// <summary>
        /// 设置执行器速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="limit">执行器速度限制，单位是RPM,该值不会超过速度量程</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setVelocityLimit(byte id, double limit, string ipAddress);

        /// <summary>
        /// 获取执行器速度限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器速度限制,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器速度限制</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVelocityLimit(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="acceleration"> Profile velocity模式的加速度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setProfileVelocityAcceleration(byte id, double acceleration, string ipAddress);

        /// <summary>
        /// 获取Profile velocity模式的加速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次Profile velocity模式的加速度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>Profile velocity模式的加速度</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getProfileVelocityAcceleration(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="deceleration">Profile velocity模式的减速度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setProfileVelocityDeceleration(byte id, double deceleration, string ipAddress);

        /// <summary>
        /// 获取Profile velocity模式的减速度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次Profile velocity模式的减速度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>Profile velocity模式的减速度</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getProfileVelocityDeceleration(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="current">目标电流，单位是A</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setCurrent(byte id, double current, string ipAddress);

        /// <summary>
        /// 获取当前电流
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电流读取,并等待返回，如果为false,则会立即返回最近一次请求电流返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前电流，单位是A</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrent(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取电流环比例
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电流环比例读取,并等待返回，如果为false,则会立即返回最近一次请求电流环比例返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前电流环比例</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrentKp(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取电流环积分
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电流环积分读取,并等待返回，如果为false,则会立即返回最近一次请求电流环积分返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前电流环积分</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrentKi(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取执行器电流量程，单位A
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器电流量程,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器电流量程</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrentRange(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 使能/失能电流环滤波功能，该功能为一阶低通滤波
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="enable">使能/失能</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void enableCurrentFilter(byte id, bool enable, string ipAddress);

        /// <summary>
        /// 读取执电流环滤波功能使能/失能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电流环滤波功能使能/失能,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>电流环滤波功能使能/失能</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isCurrentFilterEnable(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取电流环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次低通滤波频率,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>电流环低通滤波频率</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrentCutoffFrequency(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置电流环低通滤波频率
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="frequency">电流环低通滤波频率</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setCurrentCutoffFrequency(byte id, double frequency, string ipAddress);

        /// <summary>
        /// 设置执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="limit">执行器电流限制,单位是A，该值不会超过电流量程</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setCurrentLimit(byte id, double limit, string ipAddress);

        /// <summary>
        /// 获取执行器电流限制
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器电流限制,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器电流限制</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getCurrentLimit(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 执行器保存当前所有参数,如果修改参数以后没有保存，失能后将丢弃参数修改
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns></returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool saveAllParams(byte id, string ipAddress);


        [DllImport("actuatorControllerd.dll")]
        public static extern void setChartFrequency(byte id, double frequency, string ipAddress);

        [DllImport("actuatorControllerd.dll")]
        public static extern double getChartFrequency(byte id, bool bRefresh, string ipAddress);

        [DllImport("actuatorControllerd.dll")]
        public static extern void setChartThreshold(byte id, double threshold, string ipAddress);

        [DllImport("actuatorControllerd.dll")]
        public static extern double getChartThreshold(byte id, bool bRefresh, string ipAddress);

        [DllImport("actuatorControllerd.dll")]
        public static extern void enableChart(byte id, bool enable, string ipAddress);

        [DllImport("actuatorControllerd.dll")]
        public static extern bool isChartEnable(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 开启图表指定通道
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="nChanne1Id">通道id（Actuator::channel_1到Actuator::channel_4）</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void openChartChannel(byte id, byte nChanne1Id, string  ipAddress);

        /// <summary>
        /// 关闭图表指定通道
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="nChanne1Id">通道id（Actuator::channel_1到Actuator::channel_4）</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void closeChartChannel(byte id, byte nChanne1Id, string ipAddress);

        /// <summary>
        /// 设置电流模式图显模式，有Actuator::IQ_CHART和Actuator::ID_CHART两种模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="mode">图显模式</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setCurrentChartMode(byte id, byte mode);

        /// <summary>
        /// 设置电流模式图显模式，有Actuator::IQ_CHART和Actuator::ID_CHART两种模式
        /// </summary>
        /// <param name="longId">执行器id</param>
        /// <param name="mode">图显模式</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setCurrentChartMode(ulong longId, byte mode);

        /// <summary>
        /// 获取执行器电压
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器电压,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器电压</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getVoltage(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取执行器堵转能量
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次执行器堵转能量,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器堵转能量，单位J</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getLockEnergy(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置执行器堵转能量
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="energy">执行器堵转能量，单位J</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setLockEnergy(byte id, double energy, string ipAddress);

        /// <summary>
        /// 获取电机温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电机温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>电机温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getMotorTemperature(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取逆变器温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次逆变器温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>逆变器温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getInverterTemperature(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 获取电机保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电机保护温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>电机保护温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getMotorProtectedTemperature(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置电机保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">电机保护温度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setMotorProtectedTemperature(byte id, double temp, string ipAddress);

        /// <summary>
        /// 获取电机恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次电机恢复温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>电机恢复温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getMotorRecoveryTemperature(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置电机恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">电机恢复温度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setMotorRecoveryTemperature(byte id, double temp, string ipAddress);

        /// <summary>
        /// 获取逆变器保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次逆变器保护温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>逆变器保护温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getInverterProtectedTemperature(byte id, byte bRefresh, string ipAddress);

        /// <summary>
        /// 设置逆变器保护温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">逆变器保护温度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setInverterProtectedTemperature(byte id, double temp, string ipAddress);

        /// <summary>
        /// 获取逆变器恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="bRefresh">是否需要刷新，如果为true，会自动请求一次逆变器恢复温度,并等待返回，如果为false,则会立即返回最近一次请求位置返回的结果</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>逆变器恢复温度,单位℃</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern double getInverterRecoveryTemperature(byte id, bool bRefresh, string ipAddress);

        /// <summary>
        /// 设置逆变器恢复温度
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="temp">逆变器恢复温度</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void setInverterRecoveryTemperature(byte id, double temp, string ipAddress);

        /// <summary>
        /// 执行器是否在线
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>在线状态</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isOnline(byte id, string ipAddress);

        /// <summary>
        /// 执行器是否已经使能
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>是否使能</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool isEnable(byte id, string ipAddress);

        /// <summary>
        /// 使能执行器心跳功能，使能后自动刷新执行器在线状态和错误（默认状态为使能）
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void enableHeartbeat(byte id, string ipAddress);

        /// <summary>
        /// 失能执行器心跳功能，失能后关闭自动刷新执行器在线状态和错误
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void disableHeartbeat(byte id, string ipAddress);

        /// <summary>
        /// 设置执行器ID
        /// </summary>
        /// <param name="id">当前执行器id</param>
        /// <param name="newID">执行器新ID</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>是否修改成功</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern bool setActuatorID(byte id, byte newID, string ipAddress);

        /// <summary>
        /// 获取执行器序列号
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>执行器序列号</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern uint getActuatorSerialNumber(byte id, string ipAddress);

        /// <summary>
        /// 获取执行器当前模式
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>当前模式</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern ActuatorMode getActuatorMode(byte id, string ipAddress);

        /// <summary>
        /// 获取执行器错误代码
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        /// <returns>错误代码</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern uint getErrorCode(byte id, string ipAddress);

        /// <summary>
        /// 重新获取属性,将请求刷新属性
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void regainAttribute(byte id, string ipAddress);

        /// <summary>
        /// 执行器掉线重连
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void reconnect(byte id, string ipAddress);

        /// <summary>
        /// 执行器错误清除
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void clearError(byte id, string ipAddress);

        /// <summary>
        /// 获取sdk版本号字符串
        /// </summary>
        /// <returns>sdk版本号字符串</returns>
        [DllImport("actuatorControllerd.dll")]
        public static extern string versionString();

        /// <summary>
        /// 获取电流速度位置的值(如果同时需要三个值，该接口效率比较高）
        /// </summary>
        /// <param name="id">执行器id</param>
        /// <param name="ipAddress">目标ip地址字符串</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void requestCVPValue(byte id, string ipAddress);

        public delegate void doubleFuncPointer(UnifiedID unified, byte id, double number);
        public delegate void stringFuncPointer(UnifiedID unified, ushort ID, string ipAddress);
        public delegate void doubleFunction(UnifiedID unified, byte id, double number);
        public delegate void stringFunction(UnifiedID unified, ushort ID, string ipAddress);

        /// <summary>
        /// 增加参数请求回调函数，当参数请求返回结果后会触发回调
        /// </summary>
        /// <param name="callback">回调函数指针</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void addParaRequestCallback(doubleFuncPointer callback);

        /// <summary>
        /// 增加参数请求回调函数，当参数请求返回结果后会触发回调
        /// </summary>
        /// <param name="callback">回调函数function</param>
        [DllImport("actuatorControllerd.dll")]
        public static extern void addParaRequestCallback(doubleFunction callback);

        /// <summary>
        /// 增加错误回调函数，当执行器发生错误后会触发回调
        /// </summary>
        /// <param name="callback">回调函数指针</param>
        /// 当失能执行器heartbeat后，即调用disableHeartbeat后将不会上报错误
        [DllImport("actuatorControllerd.dll")]
        public static extern void addErrorCallback(stringFuncPointer callback);

        /// <summary>
        /// 增加错误回调函数，当执行器发生错误后会触发回调
        /// </summary>
        /// <param name="callback">回调函数function</param>
        /// 当失能执行器heartbeat后，即调用disableHeartbeat后将不会上报错误
        [DllImport("actuatorControllerd.dll")]
        public static extern void addErrorCallback(stringFunction callback);

        /// <summary>
        /// 清除所有回调
        /// </summary>
        [DllImport("actuatorControllerd.dll")]
        public static extern void clearCallbackHandlers();






    }
}
