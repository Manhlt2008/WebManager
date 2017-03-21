using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class RecordInfo
    {
        public DateTime? CreateDate { set; get; }
        public DateTime? ModifyDate { set; get; }
        public int CreateBy { set; get; }
        public int ModifyBy { set; get; }
    }
}
