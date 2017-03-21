using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public ChatRoom ChatRoomCheckExisted(int fromUserId, int toUserId)
        {
            var cmd = _db.CreateCommand("ChatRooms_GetByUsers", true);
            _db.AddParameter(cmd, "FromUserId", DbType.Int32, fromUserId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int32, toUserId);
            return GetList<ChatRoom>(cmd).SingleOrDefault();
        }

        public List<ChatRoom> ChatRoomGetByUserId(int userId)
        {
            var cmd = _db.CreateCommand("ChatRooms_GetByUserId", true);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);
            return GetList<ChatRoom>(cmd);
        }

        public List<ChatRoom> ChatRoomSearchs(DateTime fromDate, DateTime toDate, int userId)
        {
            var cmd = _db.CreateCommand("ChatRooms_SearchAll", true);
            _db.AddParameter(cmd, "FromDate", DbType.DateTime, fromDate);
            _db.AddParameter(cmd, "ToDate", DbType.DateTime, toDate);
            _db.AddParameter(cmd, "UserId", DbType.Int32, userId);

            return GetList<ChatRoom>(cmd);
        }

        public ChatRoom ChatRoomGetById(int roomId)
        {
            IDbCommand cmd = _db.CreateCommand("ChatRooms_GetById", true);
            _db.AddParameter(cmd, "RoomId", DbType.Int32, roomId);
            return GetList<ChatRoom>(cmd).FirstOrDefault();
        }

        public int ChatRoomInsert(ChatRoom chatRoom)
        {
            var cmd = _db.CreateCommand("ChatRooms_Insert", true);

            _db.AddParameter(cmd, "FromUserId", DbType.Int64, chatRoom.FromUserId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int64, chatRoom.ToUserId);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, chatRoom.CreateDate);

            _db.AddParameter(cmd, "Id", DbType.Int32, chatRoom.Id, ParameterDirection.Output);


            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                return (int)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }
            
            return 0;
        }
    }
}
