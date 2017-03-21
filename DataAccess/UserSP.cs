using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {

        public List<User> UserSearch(string username, string fullName, int departmentId)
        {
            var cmd = _db.CreateCommand("Users_Search", true);
            _db.AddParameter(cmd, "UserName", DbType.String, username);
            _db.AddParameter(cmd, "FullName", DbType.String, fullName);
            _db.AddParameter(cmd, "DepartmentId", DbType.Int32, departmentId);
            return GetList<User>(cmd);
        }

        public List<User> UserGetByDepartmentId(int deparmentId)
        {
            var cmd = _db.CreateCommand("Users_GetByDepartmentId", true);
            _db.AddParameter(cmd, "DepartmentId", DbType.Int32, deparmentId);
            return GetList<User>(cmd);
        }
        public List<User> UserGetAll()
        {
            var cmd = _db.CreateCommand("Users_GetAll", true);
            return GetList<User>(cmd);
        }
        public User UserGetById(int userId)
        {
            var cmd = _db.CreateCommand("Users_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, userId);
            var users = GetList<User>(cmd);
            return users == null ? null : users.FirstOrDefault();
        }
        public void UserChangePassWord(int id,string password)
        {
            var cmd = _db.CreateCommand("Users_UpdatePassword", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            _db.AddParameter(cmd, "Password", DbType.String, password);
            cmd.ExecuteNonQuery();
        }
        public User UserLogin(string userName, string password)
        {
            var cmd = _db.CreateCommand("Users_Login", true);
            _db.AddParameter(cmd, "UserName", DbType.String, userName);
            _db.AddParameter(cmd, "Password", DbType.String, password);
            var users = GetList<User>(cmd);
            return users == null ? null : users.FirstOrDefault();
        }
        public void UserCreate(string userName, string passWord, string firstName, string lastName, string address, DateTime dateOfBirth, bool gender, int deparmentId,string email,string mission, string avatar, bool isActive, bool isAdmin,bool isManager, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            var cmd = _db.CreateCommand("Users_Insert", true);
            _db.AddParameter(cmd, "UserName", DbType.String, userName);
            _db.AddParameter(cmd, "Password", DbType.String, passWord);
            _db.AddParameter(cmd, "FirstName", DbType.String, firstName);
            _db.AddParameter(cmd, "LastName", DbType.String, lastName);
            _db.AddParameter(cmd, "Address",DbType.String,address);
            _db.AddParameter(cmd, "DateOfBirth", DbType.Date, dateOfBirth);
            _db.AddParameter(cmd, "Gender", DbType.Byte, gender);
            _db.AddParameter(cmd, "DepartmentId", DbType.Int32, deparmentId);
            _db.AddParameter(cmd, "Email", DbType.String, email);
            _db.AddParameter(cmd, "Mission", DbType.String, mission);
            _db.AddParameter(cmd, "Avatar",DbType.String,avatar);
            _db.AddParameter(cmd, "IsActive",DbType.Byte,isActive);
            _db.AddParameter(cmd, "IsAdmin", DbType.Byte, isAdmin);
            _db.AddParameter(cmd, "IsManager", DbType.Byte, isManager);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, createDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, modifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, createBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, modifyBy);
            cmd.ExecuteNonQuery();
        }
        public void UserUpdate(int id, string userName, string passWord, string firstName, string lastName, string address, DateTime dateOfBirth, bool gender, int deparmentId, string email, string mission, string avatar, bool isActive, bool isAdmin, bool isManager, DateTime? createDate, DateTime? modifyDate, int createBy, int modifyBy)
        {
            var cmd = _db.CreateCommand("Users_Update", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            _db.AddParameter(cmd, "UserName", DbType.String, userName);
            _db.AddParameter(cmd, "Password", DbType.String, passWord);
            _db.AddParameter(cmd, "FirstName", DbType.String, firstName);
            _db.AddParameter(cmd, "LastName", DbType.String, lastName);
            _db.AddParameter(cmd, "Address", DbType.String, address);
            _db.AddParameter(cmd, "DateOfBirth", DbType.Date, dateOfBirth);
            _db.AddParameter(cmd, "Gender", DbType.Byte, gender);
            _db.AddParameter(cmd, "DepartmentId", DbType.Int32, deparmentId);
            _db.AddParameter(cmd, "Email", DbType.String, email);
            _db.AddParameter(cmd, "Mission", DbType.String, mission);
            _db.AddParameter(cmd, "Avatar", DbType.String, avatar);
            _db.AddParameter(cmd, "IsActive", DbType.Byte, isActive);
            _db.AddParameter(cmd, "IsAdmin", DbType.Byte, isAdmin);
            _db.AddParameter(cmd, "IsManager", DbType.Byte, isManager);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, createDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, modifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, createBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, modifyBy);
            cmd.ExecuteNonQuery();
        }
        public void UserDelete(int userId)
        {
            var cmd = _db.CreateCommand("Users_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, userId);
            cmd.ExecuteNonQuery();
        }
    }
}
