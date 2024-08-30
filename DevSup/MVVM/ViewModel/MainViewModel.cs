using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public HomeVIewModel        HomeVM { get; set; } 
        public TableSearchViewModel TableSearchVM { get; set; }


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

        public RelayCommand HomeVIewCommand { get; set; }
        public RelayCommand TableSearchViewCommand { get; set; }

        public MainViewModel()
        {
            HomeVM = new HomeVIewModel();
            TableSearchVM = new TableSearchViewModel();

            CurrentView = HomeVM;

       
            HomeVIewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });
            TableSearchViewCommand = new RelayCommand(o =>
            {
                CurrentView = TableSearchVM;
            });
        }
    }
}
