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
    /// ThemeSettingView.xaml에 대한 상호 작용 논리
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
    public partial class ThemeSettingView : UserControl
    {
        XmlLoad xmlLoad = XmlLoad.make();

        public ThemeSettingView()
        {
            InitializeComponent();


            string themeFileName = xmlLoad.GetTheme();

            // 콤보박스 항목 추가
            comboBoxThemes.Items.Add("ColorsDark.xaml");
            comboBoxThemes.Items.Add("ColorsLight.xaml");
            comboBoxThemes.Items.Add("ColorsDarkInverted.xaml");

            // 테마 이름이 콤보박스 항목 중 하나와 일치하면 선택하기
            if (themeFileName != null)
            {
                comboBoxThemes.SelectedItem = themeFileName;
            }

        }

        private void ComboBoxThemes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // 선택된 항목을 가져오기
            string selectedTheme = comboBoxThemes.SelectedItem as string;

            if (selectedTheme != null)
            {
                xmlLoad.SaveTheme(selectedTheme);
                var app = Application.Current as App;
                if (app != null)
                {

                    app.ChangeTheme(selectedTheme);
                }
            }
        }

    }
}
