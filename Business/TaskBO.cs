using System.Collections.Generic;
using DataAccess;
using Entity;
using System;

namespace Business
{
    public class TaskBO
    {
        public static List<Task> Search(string username, int leader, DateTime startDate, DateTime endDate)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.TaskSearch(username, leader, startDate, endDate);
            }
        }
        public static List<AssignTask> AssignSearch(string requirement, int userid, DateTime startDate, DateTime endDate)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AssignTaskSearch(requirement, userid, startDate, endDate);
            }
        }
        public static List<Task> GetAll()
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.TaskGetAll();
            }
        }

        public static void Insert(Task task)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.TaskInsert(task);
            }
        }

        public static void Update(Task task)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.TaskUpdate(task);
            }
        }

        public static Task GetById(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.TaskGetById(id);
            }
        }

        public static List<AssignTask> AssignTaskGetByTaskId(int taskId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AssignTaskGetByTaskId(taskId);
            }
        }

        public static void AssignTaskInsert(AssignTask assignTask)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AssignTaskInsert(assignTask);
            }
        }

        public static void AssignTaskUpdate(AssignTask assignTask)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AssignTaskUpdate(assignTask);
            }
        }

        public static void AssignTaskUpdateCompleted(int assignTaskId, int completedPercent)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AssignTaskUpdateCompleted(assignTaskId, completedPercent);
            }
        }

        public static void ReportInsert(Report report)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ReportInsert(report);
            }
        }

        public static void ReportUpdate(Report report)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ReportUpdate(report);
            }
        }

        public static void TaskDelete(int id)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.TaskDelete(id);
            }
        }

        public static void AssignTaskDelete(int id)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AssignTaskDelete(id);
            }
        }

        public static AssignTask AssignTaskGetById(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AssignTaskGetById(id);
            }
        }

        public static void ReportDelete(int id)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ReportDelete(id);
            }
        }

        public static void DocumentInsert(Document document)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.DocumentInsert(document);
            }
        }

        public static List<Document> DocumentGetByIds(List<int> ids)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.DocumentGetByIds(ids);
            }
        }
        public static List<User> ReportsOverDues(int department, string date)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ReportsOverDues(department, date);
            }
        }
        public static List<Report> ReportByUserReportAndReportDate(int userReport, string date)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ReportByUserReportAndReportDate(userReport, date);
            }
        }

        public static Report ReportGetById(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ReportGetById(id);
            }
        }
        public static List<AssignTask> AssignTaskSearch(string requirement, int userid, DateTime startDate, DateTime endDate)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AssignTaskSearch(requirement, userid, startDate, endDate);
            }
        }

        public static List<Report> ReportSearch(int userId, DateTime? startDate, DateTime? enddate)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ReportSearch(userId, startDate, enddate);
            }
        }
    }
}
