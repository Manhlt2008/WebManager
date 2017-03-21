using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using TaskManager.Utils;

namespace TaskManager.Models
{
    public class ProjectModel
    {
        public int Id { set; get; }

        [DisplayName("Project Name")]
        public string Name { set; get; }

        [DisplayName("Description")]
        public string Description { set; get; }

        [DisplayName("Start Date And End Date")]
        public string StartAndEndDate { set; get; }
    
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
    }
    public class ProjectDisplayModel : ProjectModel
    {
        public string CreatedDate { get; set; }
    }
    public class ProjectSearchModel
    {
        [DisplayName("Project Name")]
        public string Name { set; get; }

        [DisplayName("Start Date")]
        public string StartDate { set; get; }

        [DisplayName("End Date")]
        public string EndDate { set; get; }

        public List<ProjectDisplayModel> Projects { set; get; }


        public ProjectSearchModel()
        {
            Projects = new List<ProjectDisplayModel>();
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(Helper.FormatDate);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString(Helper.FormatDate);
        }
    }
}