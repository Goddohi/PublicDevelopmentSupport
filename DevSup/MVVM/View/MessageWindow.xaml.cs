﻿using DevSup.Core;
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
            //맨위에 항상 있게 해줌   xaml 에서 Topmost = "True" 
            this.Topmost = true;
            // 창이 화면의 중앙에 위치하도록 설정
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {// ESC,엔터 키를 눌렀을 때 창을 닫음
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {

                this.Close();
            }
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
            if (!IsVisible)
            {
                messageBox.Text = message;
                ShowDialog();
            }
            else
            {
                // 만약 이미 있는데 새로운창이 뜰경우 (혹시나 예외처리)
                messageBox.Text += message;
            }
        }
    }
}
