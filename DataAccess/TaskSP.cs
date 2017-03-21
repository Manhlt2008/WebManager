using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public Task TaskGetById(int id)
        {
            var cmd = _db.CreateCommand("Tasks_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            var tasks = GetList<Task>(cmd);
            return tasks == null ? null : tasks.FirstOrDefault();
        }
        public List<Task> TaskSearch(string username, int leader, DateTime startDate, DateTime endDate)
        {
            var cmd = _db.CreateCommand("Tasks_Search", true);
            _db.AddParameter(cmd, "Name", DbType.String, username);
            _db.AddParameter(cmd, "Leader", DbType.Int32, leader);
            _db.AddParameter(cmd, "StartDate", DbType.Date, startDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, endDate);
            return GetList<Task>(cmd);
        }

        public List<Task> TaskGetAll()
        {
            var cmd = _db.CreateCommand("Tasks_GetAll", true);
            return GetList<Task>(cmd);
        }

        public void TaskInsert(Task task)
        {
            var cmd = _db.CreateCommand("Tasks_Insert", true);
            _db.AddParameter(cmd, "ProjectId", DbType.Int32, task.ProjectId);
            _db.AddParameter(cmd, "Name", DbType.String, task.Name);
            _db.AddParameter(cmd, "Leader", DbType.Int32, task.Leader);
            _db.AddParameter(cmd, "Priority", DbType.Int32, task.Priority);
            _db.AddParameter(cmd, "Description", DbType.String, task.Description);
            _db.AddParameter(cmd, "StartDate", DbType.Date, task.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, task.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, task.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, task.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, task.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, task.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int32, task.Id, ParameterDirection.Output);
            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                task.Id = (int)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }
        }

        public void TaskUpdate(Task task)
        {
            var cmd = _db.CreateCommand("Tasks_Update", true);
            _db.AddParameter(cmd, "Id", DbType.String, task.Id);
            _db.AddParameter(cmd, "ProjectId", DbType.Int32, task.ProjectId);
            _db.AddParameter(cmd, "Name", DbType.String, task.Name);
            _db.AddParameter(cmd, "Leader", DbType.Int32, task.Leader);
            _db.AddParameter(cmd, "Priority", DbType.Int32, task.Priority);
            _db.AddParameter(cmd, "Description", DbType.String, task.Description);
            _db.AddParameter(cmd, "StartDate", DbType.Date, task.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, task.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, task.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, task.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, task.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, task.ModifyBy);
            cmd.ExecuteNonQuery();
        }

        public List<AssignTask> AssignTaskGetByTaskId(int taskId)
        {
            var cmd = _db.CreateCommand("AssignTasks_GetByTaskId", true);
            _db.AddParameter(cmd, "TaskId", DbType.Int32, taskId);
            return GetList<AssignTask>(cmd);
        }

        public void AssignTaskInsert(AssignTask assignTask)
        {
            var cmd = _db.CreateCommand("AssignTasks_Insert", true);
            _db.AddParameter(cmd, "TaskId", DbType.Int32, assignTask.TaskId);
            _db.AddParameter(cmd, "UserId", DbType.Int32, assignTask.UserId);
            _db.AddParameter(cmd, "Requirement", DbType.String, assignTask.Requirement);
            _db.AddParameter(cmd, "CompletedPercent", DbType.Int32, assignTask.CompletedPercent);
            _db.AddParameter(cmd, "StartDate", DbType.DateTime, assignTask.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.DateTime, assignTask.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, assignTask.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, assignTask.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, assignTask.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, assignTask.ModifyBy);
            cmd.ExecuteNonQuery();
        }

        public void AssignTaskUpdate(AssignTask assignTask)
        {
            var cmd = _db.CreateCommand("AssignTasks_Update", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, assignTask.Id);
            _db.AddParameter(cmd, "TaskId", DbType.Int32, assignTask.TaskId);
            _db.AddParameter(cmd, "UserId", DbType.Int32, assignTask.UserId);
            _db.AddParameter(cmd, "Requirement", DbType.String, assignTask.Requirement);
            _db.AddParameter(cmd, "CompletedPercent", DbType.Int32, assignTask.CompletedPercent);
            _db.AddParameter(cmd, "StartDate", DbType.DateTime, assignTask.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.DateTime, assignTask.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, assignTask.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, assignTask.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, assignTask.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, assignTask.ModifyBy);
            cmd.ExecuteNonQuery();
        }

        public void AssignTaskUpdateCompleted(int assignTaskId, int completedPercent)
        {
            var cmd = _db.CreateCommand("AssignTasks_UpdateCompleted", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, assignTaskId);
            _db.AddParameter(cmd, "CompletedPercent", DbType.Int32, completedPercent);
            cmd.ExecuteNonQuery();
        }

        public void ReportInsert(Report report)
        {
            var cmd = _db.CreateCommand("Reports_Insert", true);
            _db.AddParameter(cmd, "AssignTaskId", DbType.Int32, report.AssignTaskId);
            _db.AddParameter(cmd, "TaskId", DbType.Int32, report.TaskId);
            _db.AddParameter(cmd, "ReportDate", DbType.DateTime, report.ReportDate);
            _db.AddParameter(cmd, "ReportResult", DbType.String, report.ReportResult);
            _db.AddParameter(cmd, "NextTask", DbType.String, report.NextTask);
            _db.AddParameter(cmd, "CompletedPercent", DbType.Int32, report.CompletedPercent);
            _db.AddParameter(cmd, "UserReport", DbType.Int32, report.UserReport);
            _db.AddParameter(cmd, "Comment", DbType.String, report.Comment);
            _db.AddParameter(cmd, "CommentBy", DbType.Int32, report.CommentBy);
            _db.AddParameter(cmd, "CommentDate", DbType.DateTime, report.CommentDate);
            _db.AddParameter(cmd, "ReportType", DbType.Int32, report.ReportType);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, report.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, report.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, report.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, report.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int32, report.Id, ParameterDirection.Output);
            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                report.Id = (int)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }
        }

        public void ReportUpdate(Report report)
        {
            var cmd = _db.CreateCommand("Reports_Update", true);
            _db.AddParameter(cmd, "AssignTaskId", DbType.Int32, report.AssignTaskId);
            _db.AddParameter(cmd, "TaskId", DbType.Int32, report.TaskId);
            _db.AddParameter(cmd, "ReportDate", DbType.DateTime, report.ReportDate);
            _db.AddParameter(cmd, "ReportResult", DbType.String, report.ReportResult);
            _db.AddParameter(cmd, "NextTask", DbType.String, report.NextTask);
            _db.AddParameter(cmd, "CompletedPercent", DbType.Int32, report.CompletedPercent);
            _db.AddParameter(cmd, "UserReport", DbType.Int32, report.UserReport);
            _db.AddParameter(cmd, "Comment", DbType.String, report.Comment);
            _db.AddParameter(cmd, "CommentBy", DbType.Int32, report.CommentBy);
            _db.AddParameter(cmd, "CommentDate", DbType.DateTime, report.CommentDate);
            _db.AddParameter(cmd, "ReportType", DbType.Int32, report.ReportType);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, report.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, report.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, report.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, report.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int32, report.Id);
            cmd.ExecuteNonQuery();
        }

        public void TaskDelete(int id)
        {
            var cmd = _db.CreateCommand("Tasks_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }

        public AssignTask AssignTaskGetById(int id)
        {
            var cmd = _db.CreateCommand("AssignTasks_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            var tasks = GetList<AssignTask>(cmd);
            return tasks == null ? null : tasks.FirstOrDefault();
        }

        public void AssignTaskDelete(int id)
        {
            var cmd = _db.CreateCommand("AssignTasks_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }

        public void ReportDelete(int id)
        {
            var cmd = _db.CreateCommand("Reports_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }

        public void DocumentInsert(Document document)
        {
            var cmd = _db.CreateCommand("Documents_Insert", true);
            _db.AddParameter(cmd, "Name", DbType.String, document.Name);
            _db.AddParameter(cmd, "FilePath", DbType.String, document.FilePath);
            _db.AddParameter(cmd, "Id", DbType.Int32, document.Id, ParameterDirection.Output);
            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                document.Id = (int)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }
        }

        public List<Document> DocumentGetByIds(List<int> ids)
        {
            var cmd = _db.CreateCommand("Documents_GetByIds", true);
            _db.AddParameter(cmd, "Ids", DbType.String, string.Join(",", ids));
            return GetList<Document>(cmd);
        }
        public List<User> ReportsOverDues(int department, string date)
        { 
            var cmd=_db.CreateCommand("Reports_OverDues",true);
            _db.AddParameter(cmd, "DepartmentId",DbType.Int32, department);
            _db.AddParameter(cmd, "Date", DbType.String, date);
            return GetList<User>(cmd);
        }
        public List<Report> ReportByUserReportAndReportDate(int userReport, string date)
        {
            var cmd = _db.CreateCommand("Reports_GetByUserReportAndReportDate", true);
            _db.AddParameter(cmd, "UserReport", DbType.Int32, userReport);
            _db.AddParameter(cmd, "Date", DbType.String, date);
            return GetList<Report>(cmd);
        }

        public Report ReportGetById(int id)
        {
            var cmd = _db.CreateCommand("Reports_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            var tasks = GetList<Report>(cmd);
            return tasks == null ? null : tasks.FirstOrDefault();
        }

        public List<Report> ReportSearch(int userId, DateTime? startDate, DateTime? enddate)
        {
            var cmd = _db.CreateCommand("Reports_Search", true);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            _db.AddParameter(cmd, "StartDate", DbType.DateTime, startDate);
            _db.AddParameter(cmd, "EndDate", DbType.DateTime, enddate);
            return GetList<Report>(cmd);
        }
        public List<AssignTask> AssignTaskGetByUserId(int userid)
        {
            var cmd = _db.CreateCommand("AssignTasks_GetByUserId", true);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userid);
            return GetList<AssignTask>(cmd);
        }
        public List<AssignTask> AssignTaskSearch(string requirement, int userid, DateTime startDate, DateTime endDate)
        {
            var cmd = _db.CreateCommand("AssignTasks_Search", true);
            _db.AddParameter(cmd, "Requirement", DbType.String, requirement);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userid);
            _db.AddParameter(cmd, "StartDate", DbType.Date, startDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, endDate);
            return GetList<AssignTask>(cmd);
        }
    }
}
