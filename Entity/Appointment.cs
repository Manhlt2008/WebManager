using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Appointment : RecordInfo
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public List<User> Attendees { set; get; }

        public User User { set; get; }
        public Appointment()
        {
            Attendees = new List<User>();
        }
    }
}
