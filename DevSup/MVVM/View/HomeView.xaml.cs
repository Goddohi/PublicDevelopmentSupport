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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// 연습삼아서 만들었는데 삭제하기 싫을 정도로 이뻐서 그냥 숨겨진 이스터에그^__^
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // 기본 브라우저에서 링크를 엽니다.
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });

            // 이벤트가 처리되었음을 표시합니다.
            e.Handled = true;
        }
    }
}
