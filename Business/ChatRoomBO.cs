using System;
using System.Collections.Generic;
using DataAccess;
using Entity;

namespace Business
{
    public class ChatRoomBO
    {
        public static ChatRoom CheckExisted(int fromUserId, int toUserId)
        {
            using (var db = new DB())
            {
                return db.StoredProcedures.ChatRoomCheckExisted(fromUserId, toUserId);
            }
        }

        public static int Insert(ChatRoom chatRoom)
        {
            int chatRoomId;
            using (var db = new DB(true))
            {
                chatRoomId = db.StoredProcedures.ChatRoomInsert(chatRoom);
            }
            if (chatRoomId > 0)
                chatRoom.Id = chatRoomId;

            return chatRoomId;
        }

        public static List<ChatRoom> GetByUserId(int userId)
        {
            using (var db = new DB())
            {
                return db.StoredProcedures.ChatRoomGetByUserId(userId);
            }
        }

        public static ChatRoom GetByRoomId(int roomId)
        {
            using (var db = new DB())
            {
                return db.StoredProcedures.ChatRoomGetById(roomId);
            }
        }

        public static List<ChatRoom> Searchs(DateTime fromDate, DateTime toDate, int userId)
        {
            using (var db = new DB())
            {
                return db.StoredProcedures.ChatRoomSearchs(fromDate, toDate, userId);
            }
        }
    }
}
