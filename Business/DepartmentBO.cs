using System.Collections.Generic;
using DataAccess;
using Entity;
using System;

namespace Business
{
    public class DepartmentBO
    {
        public static List<Department> GetAll()
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.DepartmentGetAll();
            }
        }
        public static Department GetById(int departmentId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.DepartmentGetById(departmentId);
            }
        }
        public static Department GetByUserId(int userId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.DepartmentGetById(userId);
            }
        }
        public static void Create(string name, int userId, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.DepartmentCreate(name, userId, createDate, modifyDate, createBy, modifyBy);
            }
        }
        public static void Update(int id, string name, int userId, DateTime createDate, DateTime modifyDate, int createBy, int modifyBy)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.DepartmentUpdate(id, name, userId, createDate, modifyDate, createBy, modifyBy);
            }
        }
        public static void Delete(int departmentId)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.DepartmentDelete(departmentId);
            }
        }
        public static Department CheckExitUserId(int userId)
        {
            using (var db = new DB())
            {
                return db.StoredProcedures.CheckExitUserId(userId);
            }
        }
    }
}
