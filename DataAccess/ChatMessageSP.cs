using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Entity;

namespace DataAccess
{
    public partial class StoredProcedures
    {
        public List<ChatMessage> ChatMessagesGetHistoriesByTop(int chatRoomId, long lastMessageId, int top)
        {
            var cmd = _db.CreateCommand("ChatMessages_GetHistoriesByTop", true);
            _db.AddParameter(cmd, "ChatRoomId", DbType.Int32, chatRoomId);
            _db.AddParameter(cmd, "LastMessageId", DbType.Int64, lastMessageId);
            _db.AddParameter(cmd, "Top", DbType.Int32, top);
            return GetList<ChatMessage>(cmd);
        }

        public List<ChatMessage> ChatMessagesGetHistories(int chatRoomId)
        {
            var cmd = _db.CreateCommand("ChatMessages_GetByRoomId", true);
            _db.AddParameter(cmd, "ChatRoomId", DbType.Int32, chatRoomId);
            return GetList<ChatMessage>(cmd);
        }

        public ChatMessage ChatMessageGetLastest(int chatRoomId)
        {
            var cmd = _db.CreateCommand("ChatMessages_GetLastestByRoomId", true);
            _db.AddParameter(cmd, "ChatRoomId", DbType.Int32, chatRoomId);
            return GetList<ChatMessage>(cmd).FirstOrDefault();
        }

        public List<ChatMessage> ChatMessageGetListByPaging(int pageIndex, int pageSize, ref int totalRecord)
        {
            var cmd = _db.CreateCommand("ChatMessages_GetByPaging", true);
            _db.AddParameter(cmd, "PageIndex", DbType.Int32, pageIndex);
            _db.AddParameter(cmd, "PageSize", DbType.Int32, pageSize);
            _db.AddParameter(cmd, "TotalRecord", DbType.Int32, totalRecord, ParameterDirection.Output);

            List<ChatMessage> chatMessageList = GetList<ChatMessage>(cmd);
            if (((IDataParameter)cmd.Parameters["@TotalRecord"]).Value != DBNull.Value)
            {
                totalRecord = (int)((IDataParameter)cmd.Parameters["@TotalRecord"]).Value;
            }
            return chatMessageList;
        }

        public ChatMessage ChatMessageGetById(long id)
        {
            var cmd = _db.CreateCommand("ChatMessages_GetById", true);
            _db.AddParameter(cmd, "Id", DbType.Int64, id);

            var chatMessageList = GetList<ChatMessage>(cmd);
            return chatMessageList.SingleOrDefault();
        }

        public long ChatMessageInsert(ChatMessage chatMessage)
        {
            IDbCommand cmd = _db.CreateCommand("ChatMessages_Insert", true);
            _db.AddParameter(cmd, "RoomId", DbType.Int32, chatMessage.RoomId);
            _db.AddParameter(cmd, "FromUserId", DbType.Int64, chatMessage.FromUserId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int64, chatMessage.ToUserId);
            _db.AddParameter(cmd, "Message", DbType.String, chatMessage.Message);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, chatMessage.CreateDate);
            _db.AddParameter(cmd, "CreateDateStamp", DbType.Int64, chatMessage.CreateDateStamp);
            _db.AddParameter(cmd, "IsRead", DbType.Boolean, chatMessage.IsRead);
            _db.AddParameter(cmd, "Status", DbType.Int32, chatMessage.Status);
            _db.AddParameter(cmd, "Id", DbType.Int64, chatMessage.Id, ParameterDirection.Output);


            ExecuteInt32(cmd);
            if (((IDataParameter)cmd.Parameters["@Id"]).Value != DBNull.Value)
            {
                return (long)((IDataParameter)cmd.Parameters["@Id"]).Value;
            }

            return 0;
        }

        public int ChatMessageUpdate(ChatMessage chatMessage)
        {
            var cmd = _db.CreateCommand("ChatMessages_Update", true);
            _db.AddParameter(cmd, "Id", DbType.Int64, chatMessage.Id);
            _db.AddParameter(cmd, "FromUserId", DbType.Int64, chatMessage.FromUserId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int64, chatMessage.ToUserId);
            _db.AddParameter(cmd, "Message", DbType.String, chatMessage.Message);
            _db.AddParameter(cmd, "CreateDate", DbType.DateTime, chatMessage.CreateDate);
            _db.AddParameter(cmd, "CreateDateStamp", DbType.Int64, chatMessage.CreateDateStamp);
            _db.AddParameter(cmd, "IsRead", DbType.Boolean, chatMessage.IsRead);
            _db.AddParameter(cmd, "Status", DbType.Int32, chatMessage.Status);
            var result = ExecuteInt32(cmd);
            return result;
        }

        public void ChatMessageUpdateIsRead(int roomId, int toUserId)
        {
            var cmd = _db.CreateCommand("ChatMessage_UpdateIsRead", true);
            _db.AddParameter(cmd, "ChatRoomId", DbType.Int32, roomId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int32, toUserId);
            cmd.ExecuteNonQuery();
        }

        public int ChatMessageDeleteById(long id)
        {
            var cmd = _db.CreateCommand("ChatMessages_DeleteById", true);
            _db.AddParameter(cmd, "Id", DbType.Int64, id);
            int result = ExecuteInt32(cmd);
            return result;
        }

        public int ChatMessageCountUnread(int chatRoomId, long toUserId)
        {
            var cmd = _db.CreateCommand("ChatMessages_CountUnreadMessage", true);
            _db.AddParameter(cmd, "ChatRoomId", DbType.Int32, chatRoomId);
            _db.AddParameter(cmd, "ToUserId", DbType.Int32, toUserId);

            return (int)ExecuteScalar(cmd);
        }
    }
}
