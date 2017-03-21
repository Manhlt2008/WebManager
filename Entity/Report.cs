using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Report : RecordInfo
    {
        public int Id { set; get; }
        public int AssignTaskId { set; get; }
        public List<Document> Documents { set; get; }
        public int TaskId { set; get; }
        public DateTime ReportDate { set; get; }
        public string ReportResult { set; get; }
        public string NextTask { set; get; }
        public int CompletedPercent { set; get; }
        public int UserReport { set; get; }
        public string Comment { set; get; }
        public int CommentBy { set; get; }
        public DateTime CommentDate { set; get; }
        public int ReportType { get; set; }

        public User User { set; get; }
        public User UserComment { set; get; }
        public Task Task { set; get; }
        public AssignTask AssignTask { set; get; }

        public Report()
        {
            Documents = new List<Document>();
        }
    }
}
