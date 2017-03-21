using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Entity;

namespace Business
{
    public class AppointmentBO
    {
        public static void Insert(Appointment appointment)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AppointmentInsert(appointment);
            }
        }

        public static void Update(Appointment appointment)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AppointmentUpdate(appointment);
            }
        }

        public static void Delete(int id)
        {
            using (var dc = new DB(true))
            {
                dc.StoredProcedures.AppointmentDelete(id);
            }
        }

        public static List<Appointment> GetByUserId(int userId)
        {
            using (var dc = new DB(true))
            {
                return dc.StoredProcedures.AppointmentGetByUserId(userId);
            }
        }

        public static List<User> GetUsers(int appointmentId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AppointmentGetUsers(appointmentId);
            }
        }

        public static Appointment GetById(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AppointmentGetById(id);
            }
        }

        public static List<AppointmentComment> GetCommentByAppointmentId(int id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.AppointmentCommentGetByAppoimentId(id);
            }
        }

        public static AppointmentComment CommentCheckExisted(int appId, int userId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.CommentCheckExisted(appId, userId);
            }
        }

        public static int CountCommentAccepted(int appId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.CountCommentAccepted(appId);
            }
        }

        public static void AppointmentCommentInsert(AppointmentComment appointment)
        {
            using (var dc = new DB())
            {
               dc.StoredProcedures.AppointmentCommentInsert(appointment);
            }
        }

    }
}
