using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableSearchRefInfo : BaseEntityObject
    {
        public string TABLE_NAME { get; set; }

        public string OWNER { get; set; }
        public string OBJ_NAME { get; set; }
        public string OBJ_TYPE { get; set; }
        public string STATUS { get; set; }
    }
}
