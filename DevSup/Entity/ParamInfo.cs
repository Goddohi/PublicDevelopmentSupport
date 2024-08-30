using DevSup.Core;

namespace DevSup.Entity
{
    public class ParamInfo :BaseEntityObject
    {
        public string PKG_NAME { get; set; }
        public string PROC_NAME { get; set; }

        public string ARGUMENT_NAME { get; set; }

        public string DATA_TYPE { get; set; }

        public string IN_OUT { get; set; }
    }
}