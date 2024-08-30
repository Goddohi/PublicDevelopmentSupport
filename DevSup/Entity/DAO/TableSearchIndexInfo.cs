using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableSearchIndexInfo : BaseEntityObject
    {
        public string OWNER { get; set; }
        public string TABLE_NAME { get; set; }
        public string INDEX_NAME { get; set; }
        public string UNIQUENESS { get; set; }
        public int COLUMN_POSITION { get; set; }
        public string COL_NAME { get; set; }
        public string COMMENTS { get; set; }
    }
}
