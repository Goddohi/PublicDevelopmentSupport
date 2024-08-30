using DevSup.Core;
using DevSup.Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableSearchInfo : BaseEntityObject
    {
        public TableSearchInfo()
        {
        }

        public TableSearchInfo(FavTableInfo favTableInfo)
        {
            this.OWNER = favTableInfo.OWNER;
            this.TABLE_NAME = favTableInfo.TABLE_NAME;
            this.TABLE_COMMENTS = favTableInfo.TABLE_COMMENTS;
        }
  
        public string OWNER { get; set; }
            public string TABLE_NAME { get; set; }
            public string TABLE_COMMENTS { get; set; }
            public int CREATED_DAYS { get; set; }
            public int MODIFY_DAYS { get; set; }
            public string KEYWORD { get; set; }

            public string EX_OWNER { get; set; }

    }
}
