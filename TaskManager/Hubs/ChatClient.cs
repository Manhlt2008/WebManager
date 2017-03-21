using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Models.ChatModels;

namespace TaskManager.Hubs
{
    public class ChatClient
    {
        private readonly Hub _hub;

        public ChatClient(Hub hub)
        {
            _hub = hub;
        }

        public void initiateChatUI(string userId, ClientChatRoom chatRoom, bool isOnline, int totalUnreadMsg = 0)
        {
            _hub.Clients.User(userId).initiateChatUI(chatRoom, isOnline, totalUnreadMsg);
        }

        public void receiveChatMessage(string userId, UserChatMessage userChatMsg, ClientChatRoom chatRoom, bool isOnline)
        {
            _hub.Clients.User(userId).receiveChatMessage(userChatMsg, chatRoom, isOnline);
        }

        public void receiveEndChatMessage(string userId, UserChatMessage userChatMsg)
        {
            _hub.Clients.User(userId).receiveEndChatMessage(userChatMsg);
        }

        public void updateOnlineContacts(ClientChatUser user)
        {
            _hub.Clients.All.updateOnlineContacts(user);
        }

        public void updateOfflineContacts(ClientChatUser user)
        {
            _hub.Clients.All.updateOfflineContacts(user);
        }

        public void sendHistoryMessages(List<UserChatMessage> userChatMsgs, ClientChatRoom chatRoom)
        {
            _hub.Clients.Caller.receiveHistoryMessage(userChatMsgs, chatRoom, DateTime.Now);
        }

        public void receiveSupporter(ClientChatUser chatUser)
        {
            _hub.Clients.Caller.receiveSupporter(chatUser);
        }

        public void receiveInboxRooms(dynamic roomDatas)
        {
            _hub.Clients.Caller.receiveInboxRooms(roomDatas);
        }

        public void deleteMessage(int roomId, long userId)
        {
            _hub.Clients.User(userId.ToString()).deleteMessage(roomId);
        }

        public void updateOnlineUsers(List<ClientChatUser> chatUsers)
        {
            _hub.Clients.Caller.updateOnlineUsers(chatUsers);
        }

        public void updateReadMessage(int roomId, long userId)
        {
            _hub.Clients.User(userId.ToString()).updateReadMessage(roomId);
        }
    }
}