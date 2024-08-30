using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.MVVM.ViewModel.Setting
{
    class SettingViewModel : ObservableObject
    {
        public SettingBaseViewModel SettingBaseVm { get; set; }

        public HomeVIewModel HomeBM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        
        public SettingViewModel()
        {
            SettingBaseVm = new SettingBaseViewModel();

            CurrentView = SettingBaseVm;


        }
    }
}