using DevSup.MVVM.ViewModel.Setting;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevSup.MVVM.View.Setting
{
    /// <summary>
    /// DB1Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DB2SettingView : UserControl
    {
        private DB2SettingViewModel _viewModel;

        public DB2SettingView()
        {
            InitializeComponent();
            _viewModel = new DB2SettingViewModel();
            DataContext = _viewModel;
        }

        //세이브버튼누르면 작동되게 해보기
        public event EventHandler SaveClicked;
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveClicked?.Invoke(this, EventArgs.Empty);
            _viewModel.SaveData();
        }


        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgdDBUser2.SelectedItem as DBUserEntity;
            if (selectedItem != null)
            {
                _viewModel.OcDB2User.Remove(selectedItem);
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {

            var newItem = new DBUserEntity(); // 적절한 기본 값 또는 초기화 필요
            _viewModel.OcDB2User.Add(newItem);
        }

    }
}
