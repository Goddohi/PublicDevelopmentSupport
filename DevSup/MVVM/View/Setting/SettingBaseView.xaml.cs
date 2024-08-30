using DevSup.Entity;
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
    /// SettingBaseView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingBaseView : UserControl
    {
        private SettingBaseViewModel _viewModel;

        public SettingBaseView()
        {
            InitializeComponent();
            _viewModel = new SettingBaseViewModel();
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
            var selectedItem = BasicSetting.SelectedItem as BasicSettingEntity;
            if (selectedItem != null)
            {
                _viewModel.OcBasicSetting.Remove(selectedItem);
            }
        }
        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {

            var newItem = new BasicSettingEntity(); // 적절한 기본 값 또는 초기화 필요
            _viewModel.OcBasicSetting.Add(newItem);
        }
    }
}




