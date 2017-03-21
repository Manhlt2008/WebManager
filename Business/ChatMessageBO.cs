using System;
using System.Collections.Generic;
using DataAccess;
using Entity;

namespace Business
{
    public class ChatMessageBO
    {
        public static List<ChatMessage> GetHistoriesByTop(int chatRoomId, long lastMessageId, int top)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ChatMessagesGetHistoriesByTop(chatRoomId, lastMessageId, top);
            }
        }

        public static ChatMessage GetLastestMessageByRoomId(int chatRoomId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ChatMessageGetLastest(chatRoomId);
            }
        }

        public static List<ChatMessage> GetHistoriesByRoomId(int chatRoomId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ChatMessagesGetHistories(chatRoomId);
            }
        }

        public static ChatMessage GetById(long id)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ChatMessageGetById(id);
            }
        }

        public static bool Insert(ChatMessage chatMessage)
        {
            try
            {
                using (var dc = new DB(true))
                {
                    chatMessage.Id = dc.StoredProcedures.ChatMessageInsert(chatMessage);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool Update(ChatMessage chatMessage)
        {
            try
            {
                using (var dc = new DB(true))
                {
                    return dc.StoredProcedures.ChatMessageUpdate(chatMessage) >= 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(long id)
        {
            try
            {
                using (var db = new DB(true))
                {
                    return db.StoredProcedures.ChatMessageDeleteById(id) >= 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void UpdateIsRead(int roomId, int toUserId)
        {
            try
            {
                using (var dc = new DB(true))
                {
                    dc.StoredProcedures.ChatMessageUpdateIsRead(roomId, toUserId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static int CountUnread(int chatRoomId, long toUserId)
        {
            using (var dc = new DB())
            {
                return dc.StoredProcedures.ChatMessageCountUnread(chatRoomId, toUserId);
            }
        }
    }
}
