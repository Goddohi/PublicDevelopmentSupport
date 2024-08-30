using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Entity.DAO
{
    public class TableSearchCommonCode : BaseEntityObject
    {
        public string TABLE_NM { get; set; }
        public string HSP_TP_CD { get; set; }

        public string COMN_GRP_CD { get; set; }

        public string COMN_GRP_NM { get; set; }

        public string COMN_GRP_CD_NM { get; set; }
        public string COMN_GRP_CD_EXPL { get; set; }
        public string UPR_COMN_GRP_CD { get; set; }
        public string TEAM_TP_CD { get; set; }
        public string SBAR_NM { get; set; }
        public string DTRL1_EXPL { get; set; }
        public string DTRL2_EXPL { get; set; }
        public string DTRL3_EXPL { get; set; }
        public string DTRL4_EXPL { get; set; }
        public string DTRL5_EXPL { get; set; }
        public string DTRL6_EXPL { get; set; }

        public string COMN_CD { get; set; }
        public string COMN_CD_NM { get; set; }
        public string COMN_CD_EXPL { get; set; }
        public int SCRN_MRK_SEQ { get; set; }
        public string USE_YN { get; set; }
        public string DTRL1_NM { get; set; }
        public string DTRL2_NM { get; set; }
        public string DTRL3_NM { get; set; }
        public string DTRL4_NM { get; set; }
        public string DTRL5_NM { get; set; }
        public string DTRL6_NM { get; set; }
        public string NEXTG_FMR_COMN_CD { get; set; }

    }
}
