using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Entity;

namespace TaskManager.Models
{
    public class DepartmentModel
    {
        public int Id { set; get; }
        [DisplayName("DepartmentNam")]
        public string Name { set; get; }
        [DisplayName("Leader")]
        public int UserId { set; get; }
        public User User { set; get; }
        public string CreatedDate { get; set; }
    }
}