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
    public class TasksModel
    {
        public int Id { set; get; }

        [DisplayName("Project Name")]
        public int ProjectId { set; get; }

        [DisplayName("Task Name")]
        public string Name { set; get; }

        [DisplayName("Leader")]
        public int Leader { set; get; }

        [DisplayName("Priority")]
        public Priority Priority { get; set; }

        [DisplayName("Description")]
        public string Description { set; get; }

        [DisplayName("Start Date And End Date")]
        public string StartAndEndDate { set; get; }

        [DisplayName("DocumentIds")]
        public string DocumentIds { set; get; }

        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }

        public List<Document> Documents { set; get; }
        public User User { set; get; }
    }
    
    public class TasksDisplayModel : TasksModel
    {
        public string LeaderName { set; get; }
        public string ProjectName { set; get; }
        public string CreatedDate { get; set; }
    }

    public class TasksSearchModel
    {
        [DisplayName("Task Name")]
        public string Name { set; get; }

        [DisplayName("Leader")]
        public int Leader { set; get; }

        [DisplayName("Start Date")]
        public string StartDate { set; get; }

        [DisplayName("End Date")]
        public string EndDate { set; get; }

        public List<TasksDisplayModel> Tasks { set; get; }

        public TasksSearchModel()
        {
            Tasks = new List<TasksDisplayModel>();
            //DateTime sdate;
            //DateTime edate;
            StartDate= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(Helper.FormatDate);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString(Helper.FormatDate);
        }
    }

    public class ReportSearchModel
    {
        [DisplayName("User")]
        public int UserId { set; get; }

        [DisplayName("Start Date")]
        public string StartDate { set; get; }

        [DisplayName("End Date")]
        public string EndDate { set; get; }

        public List<Report> Reports { set; get; }

        public ReportSearchModel()
        {
            Reports = new List<Report>();
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(Helper.FormatDate);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString(Helper.FormatDate);
        }
    }

    public class AssignTasksSearchModel
    {
        [DisplayName("Requirement")]
        public string Requirement { set; get; }

        [DisplayName("UserId")]
        public int UserId { set; get; }

        [DisplayName("Start Date")]
        public string StartDate { set; get; }

        [DisplayName("End Date")]
        public string EndDate { set; get; }

        public List<AssignTask> AssignTask { set; get; }

        public AssignTasksSearchModel()
        {
            AssignTask = new List<AssignTask>();
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(Helper.FormatDate);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString(Helper.FormatDate);
        }
    }
}