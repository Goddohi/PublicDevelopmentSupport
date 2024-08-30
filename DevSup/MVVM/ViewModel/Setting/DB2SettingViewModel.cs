using DevSup.Entity;
using DevSup.MVVM.Model;
using DevSup.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using static DevSup.MVVM.Model.XmlLoad;

namespace DevSup.MVVM.ViewModel.Setting
{
    class DB2SettingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DBUserEntity> _ocDB2User;

        public ObservableCollection<DBUserEntity> OcDB2User
        {
            get { return _ocDB2User; }
            set
            {
                if (_ocDB2User != value)
                {
                    _ocDB2User = value;
                    OnPropertyChanged(nameof(OcDB2User));
                }
            }
        }

        // XmlLoad xmlLoad = new XmlLoad();
        XmlLoad xmlLoad = XmlLoad.make();

        public DB2SettingViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
           
            ConfigDBData dbUserEntityList;

            string filePath = xmlLoad.GetConfigXmlFilePath("DBSetting.xml");
            XmlSerializer xs = new XmlSerializer(typeof(ConfigDBData));

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    try
                    {
                        dbUserEntityList = (ConfigDBData)xs.Deserialize(reader);
                        OcDB2User = dbUserEntityList.DB2Settings; // DB2 데이터 로드
                    }
                    catch (InvalidOperationException ex)
                    {
                        // XML 파일 형식 문제에 대한 예외 처리
                        MessageWindow.Instance.ShowMessage($"데이터 로드 중 오류가 발생했습니다: {ex.Message}");
                        //MessageBox.Show($"데이터 로드 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                OcDB2User = new ObservableCollection<DBUserEntity>(); // 데이터가 없을 경우 빈 컬렉션
            }
        }

        public void SaveData()
        {
            xmlLoad.SaveDBSetting(OcDB2User, 2);
            MessageWindow.Instance.ShowMessage("데이터가 저장되었습니다.");
         //  MessageBox.Show("데이터가 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}