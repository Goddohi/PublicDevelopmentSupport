using DevSup.Core;
using DevSup.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DevSup.MVVM.View.Setting
{
    /// <summary>
    /// SettingView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingView : WinBase
    {

        private int _clickCount = 0;
        private const int DoubleClickThreshold = 500; // 더블 클릭을 감지하기 위한 시간 (밀리초)
        public event EventHandler SettingViewClosed;
        XmlLoad xmlLoad = XmlLoad.make();
        public SettingView(EventAggregator eventAggregator)
        {
            InitializeComponent();
            //닫혔을때 인식하는 이벤트 구독
            this.Closed += SettingView_Closed;
            //이스터에그 테스트
            _eventAggregator = eventAggregator;
            //Tab설정 저장버튼 이벤트 구독(제작중)
            TabUsedSetting.SaveClicked += TabUsedSetting_SaveClicked;
        }



        private void TabUsedSetting_SaveClicked(object sender, EventArgs e)
        {
            Console.WriteLine("눌리는지 테스트 ");
            this.DialogResult = true;
        }

        private void SettingView_Closed(object sender, EventArgs e)
        {
            // 창이 닫힐 때 SettingViewClosed 이벤트를 발생시킴
            SettingViewClosed?.Invoke(this, EventArgs.Empty);
        }



  
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Maximize_window();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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


        private void Setting_Loaded(object sender, RoutedEventArgs e)
        {
            MainLogo();
            //탭 로드하기
            xmlLoad.AddTab(SettingControl,"0", "SettingBaseView","Setting");
            xmlLoad.AddTab(SettingControl, "1", "DB1SettingView", "Setting");
            xmlLoad.AddTab(SettingControl, "2", "DB2SettingView", "Setting");
            xmlLoad.AddTab(SettingControl, "3", "TabUsedSettingView","Setting"); 
            SettingControl.SelectedIndex = 0;

        }

        

        private void Tab0_Click(object sender, RoutedEventArgs e)
        {
            SettingControl.SelectedIndex = 0;
        }
        private void Tab1_Click(object sender, RoutedEventArgs e)
        {
            SettingControl.SelectedIndex = 1;
        }
        private void Tab2_Click(object sender, RoutedEventArgs e)
        {
            SettingControl.SelectedIndex = 2;
        }


        private int egeclickCount = 0;
        private Stopwatch stopwatch = new Stopwatch();
        private EventAggregator _eventAggregator = new EventAggregator();
        private bool egg = false;

        private void Tab3_Click(object sender, RoutedEventArgs e)
        {
            SettingControl.SelectedIndex = 3;
            if (!egg)
            {
                // 스톱워치가 실행 중이 아니면 시작
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                }

                // 클릭 수 증가
                egeclickCount++;

                // 버튼이 3초 이내에 5번 클릭되었는지 확인
                if (stopwatch.Elapsed.TotalSeconds <= 3)
                {
                    if (egeclickCount >= 5)
                    {
                        // 특별한 이벤트 처리
                        _eventAggregator.PublishSpecialEvent();
                        //MessageBox.Show("이스터에그발동");
                        MessageWindow.Instance.ShowMessage("이스터에그발동");
                        // 클릭 수와 스톱워치 리셋
                        egeclickCount = 0;
                        stopwatch.Reset();
                        egg = true;
                    }
                }
                else
                {
                    // 3초가 초과되면 클릭 수와 스톱워치 리셋
                    egeclickCount = 1;  // 현재 클릭은 카운트됨
                    stopwatch.Restart();
                }
            }
        }

    }
}
