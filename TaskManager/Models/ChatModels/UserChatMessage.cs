using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models.ChatModels
{
    public class UserChatMessage
    {
        public string chatMessageId { get; set; }
        public int chatRoomId { get; set; }
        public int productId { get; set; }
        public string senderId { get; set; }
        public string senderName { get; set; }
        public string messageText { get; set; }

        public bool isRead { get; set; }
        public long timestamp { get; set; }
    }
}