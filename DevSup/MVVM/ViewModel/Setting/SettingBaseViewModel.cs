using DevSup.Entity;
using DevSup.MVVM.Model;
using DevSup.MVVM.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace DevSup.MVVM.ViewModel.Setting
{
    public class SettingBaseViewModel : INotifyPropertyChanged
    {
        //private XmlLoad xmlLoad = new XmlLoad();

        XmlLoad xmlLoad = XmlLoad.make();
        private ObservableCollection<BasicSettingEntity> _ocBasicSetting = new ObservableCollection<BasicSettingEntity>();

        public ObservableCollection<BasicSettingEntity> OcBasicSetting
        {
            get { return _ocBasicSetting; }
            set
            {
                if (_ocBasicSetting != value)
                {

                    _ocBasicSetting = value;
                    OnPropertyChanged(nameof(OcBasicSetting));
                }
            }
        }

        public SettingBaseViewModel()
        {
            _ocBasicSetting = new ObservableCollection<BasicSettingEntity>();   LoadData();
        }

        private void LoadData()
        {
            try
            {
                OcBasicSetting = xmlLoad.GetBasicSetting();
            }
            catch (Exception ex)
            {
                MessageWindow.Instance.ShowMessage($"데이터를 로드하는 중 오류가 발생했습니다: {ex.Message}");
                //MessageBox.Show($"데이터를 로드하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveData()
        {
            try
            {
                xmlLoad.SaveBasicSetting(OcBasicSetting);
                MessageWindow.Instance.ShowMessage("데이터가 저장되었습니다.");
               //MessageBox.Show("데이터가 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageWindow.Instance.ShowMessage($"데이터를 저장하는 중 오류가 발생했습니다: {ex.Message}");
                //MessageBox.Show($"데이터를 저장하는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
