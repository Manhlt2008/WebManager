using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using TaskManager.Models.ChatModels;

namespace TaskManager.Utils
{
    public static class ChatMessageExtension
    {
        public static UserChatMessage ToUserChatMessage(this ChatMessage chatMessage)
        {
            return new UserChatMessage
                {
                    chatMessageId = chatMessage.Id.ToString(),
                    chatRoomId = chatMessage.RoomId,
                    isRead = chatMessage.IsRead,
                    messageText = chatMessage.Message,
                    senderId = EncryptUtils.EncryptObject(chatMessage.FromUserId),
                    timestamp = chatMessage.CreateDateStamp
                };
        }
    }
}