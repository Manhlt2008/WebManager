using System.Collections.Generic;
using DataAccess;
using Entity;
using System;
namespace Business
{
    public class UserBO
    {
        public static List<User> Search(string username, string fullName, int departmentId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.UserSearch(username, fullName, departmentId);
            }
        }
        public static List<User> GetByDepartmentId(int departmentId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.UserGetByDepartmentId(departmentId);
            }
        }
        public static User GetById(int userId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.UserGetById(userId);
            }
        }

        public static List<User> GetAll()
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.UserGetAll();
            }
        }
        public static User Login(string username, string password)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.UserLogin(username, password);
            }
        }
        public static void Create(string userName, string passWord, string firstName, string lastName, string address, DateTime dateOfBirth, bool gender, int deparmentId, string email, string mission, string avatar, bool isActive, bool isAdmin,bool isManager, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.UserCreate(userName, passWord, firstName, lastName, address, dateOfBirth, gender, deparmentId, email,mission,avatar, isActive, isAdmin,isManager, createDate, modifyDate, createBy, modifyBy);
            }
        }
        public static void Update(int id,string userName, string passWord, string firstName, string lastName, string address, DateTime dateOfBirth, bool gender, int deparmentId,  string email, string mission,string avatar, bool isActive, bool isAdmin,bool isManager, DateTime? createDate, DateTime? modifyDate, int createBy, int modifyBy)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.UserUpdate(id, userName, passWord, firstName, lastName, address, dateOfBirth, gender, deparmentId,email,mission, avatar, isActive, isAdmin,isManager, createDate, modifyDate, createBy, modifyBy);
            }
        }
        public static void Delete(int userId)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.UserDelete(userId);
            }
        }
        public static void ChangePassword(int id, string password)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.UserChangePassWord(id, password);
            }
        }
    }
}
