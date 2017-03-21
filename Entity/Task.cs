using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Task : RecordInfo
    {
        public int Id { set; get; }
        public int ProjectId { get; set; }
        public string Name { set; get; }
        public int Leader { set; get; }
        public Priority Priority { get; set; }
        public string Description { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public List<Document> Documents { set; get; }
        public User User { set; get; }

        public Task()
        {
            Documents = new List<Document>();
        }
    }

    public enum Priority 
    {
        [Description("Low")]
        Low,

        [Description("Normal")]
        Normal,

        [Description("High")]
        High,

    }
}
