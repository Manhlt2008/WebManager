using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Alert
    {
        public long Id { set; get; }
        public string Message { set; get; }
        public int UserId { set; get; }
        public int ReferenceId { set; get; }
        public int TypeId { set; get; }
        public bool IsRead { set; get; }
    }

    public enum AlertType
    {
        Event,

        Task,

        AssignTask,

        Report,

        Comment
    }
}
