using DevSup.Entity;
using DevSup.MVVM.Model;
using DevSup.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevSup.MVVM.ViewModel.Setting
{
    class TabUsedSettingViewModel : INotifyPropertyChanged
    {


        XmlLoad xmlLoad = XmlLoad.make();
        private ObservableCollection<TabSettingEntity> _ocTabSetting;

        public ObservableCollection<TabSettingEntity> OcTabSetting
        {
            get { return _ocTabSetting; }
            set
            {
                if (_ocTabSetting != value)
                {
                    _ocTabSetting = value;
                    OnPropertyChanged(nameof(OcTabSetting));
                }
            }
        }

        public TabUsedSettingViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
           // XmlLoad xmlLoad = new XmlLoad();
            OcTabSetting = xmlLoad.GetTabSetting();
   
        }

        public void SaveData()
        {
            //XmlLoad xmlLoad = new XmlLoad();
            xmlLoad.SaveTabSetting(OcTabSetting);
            MessageWindow.Instance.ShowMessage("데이터가 저장되었습니다.");
           //MessageBox.Show("데이터가 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
