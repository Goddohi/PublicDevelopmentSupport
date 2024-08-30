using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DTO
{
    public class TableSearchSqlDTO : BaseEntityObject
    {
        public string SQLNAME { get; set; }
        public string QUERY { get; set; }
        public string COMMENT { get; set; }

    }
}
