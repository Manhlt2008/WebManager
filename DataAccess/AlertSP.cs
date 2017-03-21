using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Entity;
using Task = System.Threading.Tasks.Task;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public List<Alert> AlertGetAll(int userId)
        {
            var cmd = _db.CreateCommand("Alerts_GetByUserId", true);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            return GetList<Alert>(cmd);
        }


        public void AlertInsert(Alert task)
        {
            var cmd = _db.CreateCommand("Alerts_Insert", true);
            _db.AddParameter(cmd, "Message", DbType.String, task.Message);
            _db.AddParameter(cmd, "UserId", DbType.Int32, task.UserId);
            _db.AddParameter(cmd, "ReferenceId", DbType.Int32, task.ReferenceId);
            _db.AddParameter(cmd, "TypeId", DbType.Int32, task.TypeId);
            _db.AddParameter(cmd, "IsRead", DbType.Boolean, task.IsRead);
            cmd.ExecuteNonQuery();
        }

        public void AlertDelete(int id)
        {
            var cmd = _db.CreateCommand("Alerts_Delete", true);
            _db.AddParameter(cmd, "Id", DbType.Int32, id);
            cmd.ExecuteNonQuery();
        }

        public void AlertDelete(int referenceId, int typeId)
        {
            var cmd = _db.CreateCommand("Alerts_DeleteByType", true);
            _db.AddParameter(cmd, "TypeId", DbType.Int32, typeId);
            _db.AddParameter(cmd, "ReferenceId", DbType.Int32, referenceId);
            cmd.ExecuteNonQuery();
        }

    }
}
