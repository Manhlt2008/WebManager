using System.Collections.Generic;
using Entity;

namespace TaskManager.Models.ChatModels
{
    public class ServerChatRoom : ChatRoom
    {
        public List<User> ChatUsers { get; set; }
        public ServerChatRoom()
        {
            ChatUsers = new List<User>();
        }
    }
}