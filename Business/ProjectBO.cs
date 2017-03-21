using System.Collections.Generic;
using DataAccess;
using Entity;
using System;

namespace Business
{
    public class ProjectBO
    {
        public static List<Project> Search(string name, DateTime startDate, DateTime endDate)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ProjectSearch(name, startDate, endDate);
            }
        }
        public static List<Project> GetAll()
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ProjectGetAll();
            }
        }
        public static void Insert(Project project)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ProjectInsert(project);
            }
        }

        public static void Update(Project project)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ProjectUpdate(project);
            }
        }

        public static Project GetById(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ProjectGetById(id);
            }
        }
        public static void ProjectDelete(int id)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.ProjectDelete(id);
            }
        }
    }
}
