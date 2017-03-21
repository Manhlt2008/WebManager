using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Entity;

namespace Business
{
    public class AlertBO
    {
        public static List<Alert> GetAll(int userId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AlertGetAll(userId);
            }
        }

        public static void Insert(Alert alert)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.AlertInsert(alert);
            }
        }

        public static void Insert(string message, int typeId, int referenceId, int userId)
        {
            switch (typeId)
            {
                case (int)AlertType.AssignTask:
                    message = "Assign Task: " + message;
                    break;
                case (int)AlertType.Task:
                    message = "Task: " + message;
                    break;
                case (int)AlertType.Comment:
                    message = "Comment for report: " + message;
                    break;
                case (int)AlertType.Event:
                    message = "Event: " + message;
                    break;
                case (int)AlertType.Report:
                    message = "Report from employee: " + message;
                    break;
            }

            var alert = new Alert
                {
                    Message = message,
                    ReferenceId = referenceId,
                    TypeId = typeId,
                    UserId = userId,
                    IsRead = false
                };

            Insert(alert);

        }

        public static void Delete(int id)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.AlertDelete(id);
            }
        }

        public static void Delete(int referenceId, int typeId)
        {
            using (var db = new DB(true))
            {
                db.StoredProcedures.AlertDelete(referenceId, typeId);
            }
        }
    }
}
