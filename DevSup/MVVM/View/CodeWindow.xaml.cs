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
using System.Windows.Threading;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// CodeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CodeWindow : Window, IDisposable
    {
        private int _clickCount = 0;
        private const int DoubleClickThreshold = 500; // 더블 클릭을 감지하기 위한 시간 (밀리초)

        public CodeWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Maximize_window()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickCount++;
            if (_clickCount == 1)
            {
                // 첫 번째 클릭 후 타이머 시작
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(DoubleClickThreshold)
                };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    _clickCount = 0; // 타이머가 만료되면 클릭 카운트 리셋
                };
                timer.Start();
            }
            else if (_clickCount == 2)
            {
                _clickCount = 0;
                Maximize_window();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 닫기 메소드
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //최대화
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Maximize_window();
         }


        private void TxtCode_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke
            (
                DispatcherPriority.ContextIdle,
                new Action
                (
                    delegate
                    {
                        (sender as TextBox).SelectAll();
                    }
                )
            );
        }

        public void Dispose()
        {

        }
    }
}
