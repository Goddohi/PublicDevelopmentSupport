using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableColumnInfo : BaseEntityObject
    {
        public int COLUMN_ID { get; set; }
        public string TABLE_NAME { get; set; }
        public string OWNER { get; set; }
        public string COL_NAME { get; set; }
        public string NULLABLE { get; set; }
        public string DATATYPE { get; set; }
        public string COMMENTS { get; set; }
        public string DEFAULT { get; set; }
        public int DATA_LENGTH { get; set; }
        public string COMMENTS2 { get; set; }
        public string COMMON_CODE { get; set; }
    }
}
