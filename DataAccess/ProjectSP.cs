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
        public Project ProjectGetById(int id)
        {
            var cmd = _db.CreateCommand("Projects_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            var project = GetList<Project>(cmd);
            return project == null ? null : project.FirstOrDefault();
        }
        public List<Project> ProjectSearch(string name, DateTime startDate, DateTime endDate)
        {
            var cmd = _db.CreateCommand("Projects_Search", true);
            _db.AddParameter(cmd, "Name", DbType.String, name);
            _db.AddParameter(cmd, "StartDate", DbType.Date, startDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, endDate);
            return GetList<Project>(cmd);
        }
        public List<Project> ProjectGetAll()
        {
            var cmd = _db.CreateCommand("Projects_GetAll", true);
            return GetList<Project>(cmd);
        }
        public void ProjectInsert(Project project)
        {
            var cmd = _db.CreateCommand("Projects_Insert", true);
            _db.AddParameter(cmd, "Name", DbType.String, project.Name);
            _db.AddParameter(cmd, "Description", DbType.String, project.Description);
            _db.AddParameter(cmd, "StartDate", DbType.Date, project.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, project.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, project.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, project.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, project.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, project.ModifyBy);
            cmd.ExecuteNonQuery();
        }
        public void ProjectUpdate(Project project)
        {
            var cmd = _db.CreateCommand("Projects_Update", true);
            _db.AddParameter(cmd, "Id", DbType.String, project.Id);
            _db.AddParameter(cmd, "Name", DbType.String, project.Name);
            _db.AddParameter(cmd, "Description", DbType.String, project.Description);
            _db.AddParameter(cmd, "StartDate", DbType.Date, project.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.Date, project.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, project.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, project.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, project.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, project.ModifyBy);
            cmd.ExecuteNonQuery();
        }

        public void ProjectDelete(int id)
        {
            var cmd = _db.CreateCommand("Projects_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }
    }
}
