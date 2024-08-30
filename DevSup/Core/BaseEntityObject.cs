using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Core
{
    public class BaseEntityObject : INotifyPropertyChanged, ICloneable
    {
        public string Mode { set; get; } //S, I, D, U

        public string DBGbn { set; get; } //DEV, APP

        public string ServiceName { set; get; }

        public string QueryId { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
