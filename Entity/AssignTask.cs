using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class AssignTask : RecordInfo
    {
        public int Id { set; get; }
        public int TaskId { set; get; }
        public int UserId { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public string Requirement { set; get; }
        public int CompletedPercent { set; get; }

        public string StartAndEndDate { set; get; }
        public User User { set; get; }
        public Task Task { set; get; }
    }
}
