using DevSup.Entity;
using DevSup.MVVM.Model;
using DevSup.MVVM.ViewModel.Setting;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DevSup.MVVM.View.Setting
{
    /// <summary>
    /// TabUsedSettingView.xaml에 대한 상호 작용 논리
    /// </summary>
    /*
    public partial class TabUsedSettingView : UserControl
    {
        private ObservableCollection<TabSettingEntity> ocTabSetting;

        public ObservableCollection<TabSettingEntity> OcTabSetting
        {
            get { return ocTabSetting; }
            set
            {
                ocTabSetting = value;
                // PropertyChanged 이벤트를 발생시켜 UI에 변경사항을 알립니다.
                OnPropertyChanged(nameof(OcTabSetting));
            }
        }

        public TabUsedSettingView()
        {
            InitializeComponent();
            // Initialize DataContext and load data
            DataContext = this;  // Set DataContext to self to bind properties
            LoadData();
        }

        private void LoadData()
        {
            XmlLoad xmlLoad = new XmlLoad();
            OcTabSetting = xmlLoad.GetTabSetting();
        }

        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            // Optionally refresh or reload data when the control is loaded
            LoadData();
        }

        // Implement INotifyPropertyChanged interface
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            XmlLoad xmlLoad = new XmlLoad();
            xmlLoad.SaveTabSetting(OcTabSetting);
           
            MessageBox.Show("데이터가 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    */
    public partial class TabUsedSettingView : UserControl
    {
        private TabUsedSettingViewModel _viewModel;

        public TabUsedSettingView()
        {
            InitializeComponent();
            _viewModel = new TabUsedSettingViewModel();
            DataContext = _viewModel;  
        }

        //세이브버튼누르면 작동되게 해보기
        public event EventHandler SaveClicked;
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
               SaveClicked?.Invoke(this, EventArgs.Empty);
            _viewModel.SaveData();
        }
    }
}
