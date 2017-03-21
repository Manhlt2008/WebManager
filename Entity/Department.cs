using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Department : RecordInfo
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public int UserId { set; get; }
        public User User { set; get; }
    }
}
