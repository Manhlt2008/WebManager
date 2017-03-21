using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class AppointmentComment : RecordInfo
    {
        public long Id { set; get; }
        public CommentType CommentType { set; get; }
        public int AppointmentId { set; get; }
        public string Comments { set; get; }

        public User User { set; get; }

        public AppointmentComment()
        {
            User = new User();
        }
    }

    public enum CommentType
    {
        Accepted,
        Rejected
    }
}
