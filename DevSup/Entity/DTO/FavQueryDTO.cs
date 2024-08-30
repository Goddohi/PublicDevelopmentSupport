using DevSup.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DTO
{
    public class FavQueryDTO :BaseEntityObject
    {

       //public string FAVVALUE { get; set; }

        private string _value;
        public string FAVVALUE
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(FAVVALUE));
                }
            }
        }
        public bool IsChecked
        {
            get { return FAVVALUE == "true"; }
            set
            {
                FAVVALUE = value ? "true" : "false";
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string QUERY_FOLDER { get; set; }
        public string QUERY_NAME { get; set; }
        public string QUERY_COMMENTS { get; set; }
        public string QUERY_TEXT { get; set; }
    }
}
