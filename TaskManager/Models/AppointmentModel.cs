using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class AppointmentModel
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }

        [DisplayName("Start Time And End Time")]
        public string StartAndEndDate { set; get; }
        public List<int> Attendees { set; get; }
        public string AttendessStr { set; get; }
    }
}