using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using TaskManager.Models.ChatModels;

namespace TaskManager.Utils
{
    public static class ChatRoomExtension
    {
        public static ClientChatRoom ToClientChatRoom(this ServerChatRoom serverChatRoom)
        {
            return new ClientChatRoom
                {
                    chatRoomId =  serverChatRoom.Id,
                    fromUserId = EncryptUtils.EncryptObject(serverChatRoom.FromUserId),
                    toUserId = EncryptUtils.EncryptObject(serverChatRoom.ToUserId),
                    chatUsers = serverChatRoom.ChatUsers == null ? new List<ClientChatUser>() : serverChatRoom.ChatUsers.Select(u=> u.ToClientChatUser()).ToList()
                };
        }

        public static ServerChatRoom ToServerChatRoom(this ChatRoom chatRoom)
        {
            return new ServerChatRoom
                {
                    Id = chatRoom.Id,
                    CreateDate = chatRoom.CreateDate,
                    FromUserId = chatRoom.FromUserId,
                    ToUserId = chatRoom.ToUserId,
                };
        }
    }
}