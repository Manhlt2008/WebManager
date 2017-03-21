using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Models.ChatModels
{
    public class ClientChatRoom
    {
        public int chatRoomId { get; set; }

        public int productId { get; set; }

        public string fromUserId { get; set; }

        public string toUserId { get; set; }

        public List<ClientChatUser> chatUsers { get; set; }

        public ClientChatRoom()
        {
            chatUsers = new List<ClientChatUser>();
        }
    }
}