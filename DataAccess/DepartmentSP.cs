using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public List<Department> DepartmentGetAll()
        {
            var cmd = _db.CreateCommand("Departments_GetAll", true);
            return GetList<Department>(cmd);
        }
        public Department DepartmentGetById(int departmentId)
        {
            var cmd = _db.CreateCommand("Departments_GetById", true);
            _db.AddParameter(cmd, "DepartmentId", DbType.Int32, departmentId);
            var departments = GetList<Department>(cmd);
            return departments == null ? null : departments.FirstOrDefault();
        }
        public Department CheckExitUserId(int userId)
        {
            var cmd = _db.CreateCommand("Departments_CheckExitUserId", true);
            _db.AddParameter(cmd,"UserId",DbType.Int32, userId);
            var departments = GetList<Department>(cmd);
            return departments == null ? null : departments.FirstOrDefault();
        }
        public void DepartmentCreate(string name, int userId, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            var cmd = _db.CreateCommand("Departments_Insert", true);
            _db.AddParameter(cmd, "Name", DbType.String, name);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, createDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, modifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, createBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, modifyBy);
            cmd.ExecuteNonQuery();
        }
        public void DepartmentUpdate(int departmentId,string name, int userId, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            var cmd = _db.CreateCommand("Departments_Update", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, departmentId);
            _db.AddParameter(cmd, "Name", DbType.String, name);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, createDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, modifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, createBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, modifyBy);
            cmd.ExecuteNonQuery();
        }
        public void DepartmentDelete(int departmentId)
        {
            var cmd = _db.CreateCommand("Departments_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, departmentId);
            cmd.ExecuteNonQuery();
        }
    }
}
