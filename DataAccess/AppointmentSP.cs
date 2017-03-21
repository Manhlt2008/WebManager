using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public void AppointmentInsert(Appointment appointment)
        {
            var cmd = _db.CreateCommand("Appointments_Insert", true);
            _db.AddParameter(cmd, "Title", DbType.String, appointment.Title);
            _db.AddParameter(cmd, "Description", DbType.String, appointment.Description);
            _db.AddParameter(cmd, "StartDate", DbType.DateTime, appointment.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.DateTime, appointment.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, appointment.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, appointment.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, appointment.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, appointment.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int32, appointment.Id, ParameterDirection.Output);
            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                appointment.Id = (int)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }

            // Insert attendee
            if (appointment.Attendees != null && appointment.Attendees.Count > 0)
            {
                for (int i = 0; i < appointment.Attendees.Count; i++)
                {
                    var cmd1 = _db.CreateCommand("Attendees_Insert", true);
                    _db.AddParameter(cmd1, "AppointmentId", DbType.Int32, appointment.Id);
                    _db.AddParameter(cmd1, "UserId", DbType.Int32, appointment.Attendees[i].Id);
                    cmd1.ExecuteNonQuery();
                }
            }
        }

        public void AppointmentUpdate(Appointment appointment)
        {
            var cmd = _db.CreateCommand("Appointments_Update", true);
            _db.AddParameter(cmd, "Title", DbType.String, appointment.Title);
            _db.AddParameter(cmd, "Description", DbType.String, appointment.Description);
            _db.AddParameter(cmd, "StartDate", DbType.DateTime, appointment.StartDate);
            _db.AddParameter(cmd, "EndDate", DbType.DateTime, appointment.EndDate);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, appointment.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, appointment.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, appointment.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, appointment.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int32, appointment.Id);
            cmd.ExecuteNonQuery();

            var cmd2 = _db.CreateCommand("Attendees_Delete", true);
            _db.AddParameter(cmd2, "AppointmentId", DbType.Int32, appointment.Id);
            cmd2.ExecuteNonQuery();

            // Insert attendee
            if (appointment.Attendees != null && appointment.Attendees.Count > 0)
            {
                for (int i = 0; i < appointment.Attendees.Count; i++)
                {
                    var cmd1 = _db.CreateCommand("Attendees_Insert", true);
                    _db.AddParameter(cmd1, "AppointmentId", DbType.Int32, appointment.Id);
                    _db.AddParameter(cmd1, "UserId", DbType.Int32, appointment.Attendees[i].Id);
                    cmd1.ExecuteNonQuery();
                }
            }
        }

        public void AppointmentDelete(int id)
        {
            var cmd = _db.CreateCommand("Appointments_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }

        public List<Appointment> AppointmentGetByUserId(int userId)
        {
            var cmd = _db.CreateCommand("Appointments_GetByUserId", true);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            return GetList<Appointment>(cmd);
        }

        public List<User> AppointmentGetUsers(int appointmentId)
        {
            var cmd = _db.CreateCommand("Attendees_GetByAppointmentId", true);
            _db.AddParameter(cmd, "AppointmentId", DbType.Int32, appointmentId);
            return GetList<User>(cmd);
        }

        public Appointment AppointmentGetById(int id)
        {
            var cmd = _db.CreateCommand("Appointments_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);

            return GetList<Appointment>(cmd).FirstOrDefault();
        }

        public List<AppointmentComment> AppointmentCommentGetByAppoimentId(int id)
        {
            var cmd = _db.CreateCommand("AppointmentComments_GetAppId", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);

            return GetList<AppointmentComment>(cmd);
        }

        public AppointmentComment CommentCheckExisted(int appId, int userId)
        {
            var cmd = _db.CreateCommand("AppointmentComment_CheckExisted", true);
            _db.AddParameter(cmd, "AppId", DbType.Int32, appId);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);

            return GetList<AppointmentComment>(cmd).FirstOrDefault();
        }

        
        public int CountCommentAccepted(int appId)
        {
            var cmd = _db.CreateCommand("AppointmentComment_CountAccepted", true);
            _db.AddParameter(cmd, "AppId", DbType.Int32, appId);

            return (int)ExecuteScalar(cmd);
        }

        public void AppointmentCommentInsert(AppointmentComment appointment)
        {
            var cmd = _db.CreateCommand("AppointmentComments_Insert", true);
            _db.AddParameter(cmd, "CommentType", DbType.Int32, appointment.CommentType);
            _db.AddParameter(cmd, "AppointmentId", DbType.Int32, appointment.AppointmentId);
            _db.AddParameter(cmd, "Comments", DbType.String, appointment.Comments);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, appointment.CreateDate);
            _db.AddParameter(cmd, "ModifyDate", DbType.DateTime, appointment.ModifyDate);
            _db.AddParameter(cmd, "CreateBy", DbType.Int32, appointment.CreateBy);
            _db.AddParameter(cmd, "ModifyBy", DbType.Int32, appointment.ModifyBy);
            _db.AddParameter(cmd, "Id", DbType.Int64, appointment.Id, ParameterDirection.Output);
            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                appointment.Id = (long)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }

        }

    }
}
