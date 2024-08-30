using DevSup.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity
{
    public class TabSettingEntity : BaseEntityObject
    {
        public string TABNAME { get; set; }
        public string UCNAME { get; set; }

        private string _value;
        public string VALUE
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(VALUE));
                }
            }
        }
        public string DETAIL { get; set; }
        public bool IsChecked
        {
            get { return VALUE == "true"; }
            set
            {
                VALUE = value ? "true" : "false";
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
