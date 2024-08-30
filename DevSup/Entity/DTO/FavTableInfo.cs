using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DTO
{
    public class FavTableInfo : BaseEntityObject
    {
        public string OWNER { get; set; }
        public string TABLE_NAME { get; set; }
        public string TABLE_COMMENTS { get; set; }
    }
    }
