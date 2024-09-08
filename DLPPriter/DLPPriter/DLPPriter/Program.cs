using MotionCard.键盘控制;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLPPriter
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕获的异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += Application_ThreadException;
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += CurrentDowain_UnhandleException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //通过应用程序名称检查是否重复打开应用程序
                string name = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(name);
                if (processes.Length > 1)
                {
                    MessageBox.Show("应用程序已运行,请勿重复打开!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    WorkSystem.AppStartupPath = Application.StartupPath;
                    WorkSystem.LoadParameters();
                    KeyboardHook hook = new KeyboardHook();
                    //hook.Start();
                    Application.Run(new MainFrom());
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDowain_UnhandleException);
                }


                
            }
            catch(Exception ex)
            {
                var strDateInfo = $"DLP多材料打印机应用程序出现未处理的异常:{DateTime.Now}\r\n";
                var str = $"{strDateInfo}异常类型:{ex.GetType().Name}\r\n异常信息:{ex.Message}\r\n{ex.StackTrace}";
                WriteLOG(str);
                MessageBox.Show($"金字塔机床应用程序发生错误,错误为:{str}", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 错误弹窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str;
            var strDateInfo = $"出现应用程序未处理的异常:{DateTime.Now}\r\n";
            var error = e.Exception;
            if (error != null)
            {
                str = $"{strDateInfo}异常类型:{error.GetType().Name}\r\n异常信息:{error.Message}\r\n异常信息:{error.StackTrace}";
            }
            else
            {
                str = $"应用程序线程错误:{e}";
            }
            WriteLOG(str);
            MessageBox.Show($"发生错误,请查看程序日志!错误为:\r\n{str}", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }


        static void CurrentDowain_UnhandleException(object sender, UnhandledExceptionEventArgs e)
        {
            string str;
            var strDateInfo = $"出现应用程序未处理的异常:{DateTime.Now}\r\n";
            var error = e.ExceptionObject as Exception;
            if (error != null)
            {
                str = $"{strDateInfo}Application UnhandledException:{error.Message}\r\n{error.StackTrace}";
            }
            else
            {
                str = $"Application UnhandledException:{e}";
            }
            WriteLOG(str);
            MessageBox.Show($"发生错误,请查看程序日志!错误为:\r\n{str}", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }


        static void WriteLOG(string str)
        {
            if (!Directory.Exists("ErrLog"))
            {
                Directory.CreateDirectory("ErrLog");
            }
            using (var sw = new StreamWriter($"Errlog//{DateTime.Now.ToString("yyyy-MM-dd")}.log", true))
            {
                sw.WriteLine(str);
                sw.WriteLine("------------------------------------------------");
                sw.Close();
            }
        }
    }
}
