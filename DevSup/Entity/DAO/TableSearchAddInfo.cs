using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableSearchAddInfo : BaseEntityObject
    {
        public string TABLE_NAME { get; set; }

        public string COL_NAME { get; set; }
        public string COL_VALUE { get; set; }
    }
}
