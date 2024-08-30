using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Data;
using DevSup.MVVM.View;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevSup.Entity;

namespace DevSup.Core
{
    public class UCBase : UserControl
    {

        protected MainWindow OwnerWindow
        {
            get
            {
                return Window.GetWindow(this) as MainWindow;
            }
        }

        public int DB { set; get; }


        public UCBase()
            : base()
        {
            this.DB = -1;
        }

        protected string SaveTempTextFile(string file_name, string txt)
        {
            string file_path = this.GetTempFilePath(file_name);

            try
            {
                File.WriteAllText(file_path, txt);
                //File.WriteAllText(file_path, txt, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageWindow.Instance.ShowMessage(ex.Message);
                //MessageBox.Show(ex.Message);
            }

            return file_path;
        }

        public string GetTempFilePath(string file_name)
        {
            string file_path = string.Format(@"Temp\{0}", file_name);

            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    var deployment = ApplicationDeployment.CurrentDeployment;
                    file_path = System.IO.Path.Combine(deployment.DataDirectory, file_path);
                }
                else
                {
                    file_path = System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), file_path);
                }

                FileInfo fi = new FileInfo(file_path);
                if (!Directory.Exists(fi.Directory.FullName))
                {
                    Directory.CreateDirectory(fi.Directory.FullName);
                }
            }
            catch (Exception ex)
            {
                // 예외 처리 로직 추가
                // 예: 로그 작성, 사용자에게 알림 등
                MessageWindow.Instance.ShowMessage($"경로 설정 중 오류가 발생했습니다: {ex.Message}");
                //MessageBox.Show($"경로 설정 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return file_path;
        }
        protected void ProgressOn()
        {
            if (OwnerWindow == null) return;
           // OwnerWindow.ProgressOn();
        }
        protected void ProgressOff()
        {
            if (OwnerWindow == null) return;
           // OwnerWindow.ProgressOff();
        }


        protected string NVL(object str, string replace_str)
        {
            return str == null ? replace_str: str.ToString() ;
        }
        /*

        private T NVL<T>(T value, T defaultValue)
        {
            return value != null ? value : defaultValue;
        }
        */

        protected void OpenCodeWIndow(string code)
        {
            using (CodeWindow window = new CodeWindow())
            {
                window.Owner = this.Parent as Window;
                window.Show();
                window.TxtCode.Text = code;
            }
        }



        protected void OpenCodeWIndow(string title,string code)
        {
            using (CodeWindow window = new CodeWindow())
            {
                window.Owner = this.Parent as Window;
                window.Show();
                window.Title = title;
                window.titlecode.Text = title;
                window.TxtCode.Text = code;
            }
        }
        /*
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        */
        public void ExecuteProgram(string path, string src_file_path)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = path;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Arguments = "\"" + src_file_path + "\"";
                if (p.Start())
                {
                    //p.WaitForInputIdle();
                    //IntPtr mainWindowHandle = p.MainWindowHandle;
                    //SetForegroundWindow(mainWindowHandle);
                }
            }
        }
    }
}
