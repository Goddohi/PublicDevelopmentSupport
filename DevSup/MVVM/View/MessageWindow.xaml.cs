using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// 메세지창이 안이뻐서 만듬
    /// </summary>
    public partial class MessageWindow : WinBase
    {
        private static MessageWindow _instance;

        private MessageWindow()
        {
            SemiLogo();
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        

        // Singleton 인스턴스에 접근하는 속성
        public static MessageWindow Instance
        {
            get
            {
                if (_instance == null || !_instance.IsVisible)
                {
                    _instance = new MessageWindow();
                }
                return _instance;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
        public void ShowMessage(string message)
        {
            messageBox.Text = message;
            ShowDialog();
        }
    }
}
