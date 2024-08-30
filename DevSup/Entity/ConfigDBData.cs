using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DevSup.Entity
{
    [XmlRoot("ConfigDBData")]
    public class ConfigDBData
    {
        [XmlArray("DB1Settings")]
        [XmlArrayItem("DBUserEntity")]
        public ObservableCollection<DBUserEntity> DB1Settings { get; set; }

        [XmlArray("DB2Settings")]
        [XmlArrayItem("DBUserEntity")]
        public ObservableCollection<DBUserEntity> DB2Settings { get; set; }

        // 기본 생성자
        public ConfigDBData()
        {
            DB1Settings = new ObservableCollection<DBUserEntity>();
            DB2Settings = new ObservableCollection<DBUserEntity>();
        }
    }
}
