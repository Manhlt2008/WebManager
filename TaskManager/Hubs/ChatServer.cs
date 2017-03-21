using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business;
using Entity;
using Microsoft.AspNet.SignalR;
using TaskManager.Models.ChatModels;
using TaskManager.Utils;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Hubs
{
    public class ChatServer : Hub
    {
        #region Variables

        public static ConcurrentDictionary<int, ServerChatRoom> _OnlineChatRooms = new ConcurrentDictionary<int, ServerChatRoom>();
        public static ConcurrentDictionary<int, User> _UserOnlines = new ConcurrentDictionary<int, User>();

        private ChatClient _client;
        private ChatClient chatClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new ChatClient(this);
                }

                return _client;
            }
        }

        #endregion

        #region override

        public override Task OnConnected()
        {
            var chatUser = Helper.CurrentUser;
            if (chatUser != null)
            {
                AddUserOnline(chatUser);
                // Update user online list
                chatClient.updateOnlineContacts(chatUser.ToClientChatUser());
            }
            else
            {
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var chatUser = Helper.CurrentUser;
            if (chatUser != null)
            {
                if (RemoveUserOnline(chatUser.Id))
                {
                    // Update user online list - Don't need
                    // chatClient.updateOfflineContacts(chatUser.ToClientChatUser());

                    // Get all other ChatUser in room to send offline
                    var clientUser = chatUser.ToClientChatUser();
                    chatClient.updateOfflineContacts(clientUser);
                }
            }

            return base.OnReconnected();
        }

        public override Task OnReconnected()
        {
            var chatUser = Helper.CurrentUser;
            if (chatUser != null)
            {
                AddUserOnline(chatUser);
                // Update user online list
                chatClient.updateOnlineContacts(chatUser.ToClientChatUser());
            }
            else
            {
            }
            return base.OnReconnected();
        }

        #endregion

        #region Public method

        public bool InitiateChat(string clientToUserId)
        {
            // Get current user
            var fromUser = Helper.CurrentUser;
            if (fromUser == null)
            {
                return false;
            }

            // Check to user is Authenticate User, can't chat to guest user
            var toUserId = EncryptUtils.Decrypt(clientToUserId);
            int lToUserId;
            int.TryParse(toUserId, out lToUserId);
            if (lToUserId <= 0) return false;
            // Get from User Online list
            bool isToUserOnline;
            User toUserOnline = null;
            if (_UserOnlines.ContainsKey(lToUserId))
            {
                toUserOnline = _UserOnlines[lToUserId];
                isToUserOnline = true;
            }
            else
            {
                isToUserOnline = false;
                // Get user from User table
                toUserOnline = UserBO.GetById(lToUserId);
            }

            // Exist online chat room
            var id1 = fromUser.Id;
            var id2 = lToUserId;
            Helper.Sort(ref id1, ref id2);
            var chatRoomExisted = ChatRoomBO.CheckExisted(id1, id2);
            var serverChatRoom = chatRoomExisted == null ? null : chatRoomExisted.ToServerChatRoom();
            // total unread message
            var totalUnreadMsg = 0;
            if (serverChatRoom == null)
            {
                var chatRoom = new ChatRoom
                {
                    FromUserId = id1,
                    ToUserId = id2,
                    CreateDate = DateTime.Now
                };
                var chatRoomId = ChatRoomBO.Insert(chatRoom);
                if (chatRoomId <= 0) return false;

                serverChatRoom = chatRoom.ToServerChatRoom();
                // Add to user to server chat rooms
                serverChatRoom.ChatUsers.Add(fromUser);
                serverChatRoom.ChatUsers.Add(toUserOnline);
                totalUnreadMsg = ChatMessageBO.CountUnread(serverChatRoom.Id, fromUser.Id);
                var clientChatRoom = serverChatRoom.ToClientChatRoom();
                chatClient.initiateChatUI(fromUser.Id.ToString(), clientChatRoom, isToUserOnline, totalUnreadMsg);
            }
            else
            {
                serverChatRoom.ChatUsers = new List<User>
                    {
                        UserBO.GetById(serverChatRoom.FromUserId),
                        UserBO.GetById(serverChatRoom.ToUserId),
                    };
                totalUnreadMsg = ChatMessageBO.CountUnread(serverChatRoom.Id, fromUser.Id);
                chatClient.initiateChatUI(fromUser.Id.ToString(), serverChatRoom.ToClientChatRoom(), isToUserOnline, totalUnreadMsg);
            }

            _OnlineChatRooms.TryAdd(serverChatRoom.Id, serverChatRoom);
            return true;
        }

        public void SendChatMessage(UserChatMessage userChatMsg)
        {
            var fromUser = Helper.CurrentUser;
            if (fromUser == null)
            {
                return;
            }

            ServerChatRoom serverChatRoom;
            _OnlineChatRooms.TryGetValue(userChatMsg.chatRoomId, out serverChatRoom);
            if (serverChatRoom != null)
            {
                var toChatUser = serverChatRoom.ChatUsers.FirstOrDefault(t => t.Id != fromUser.Id);
                if (toChatUser == null) return;
                var datetime = DateTime.Now;
                var dateStamp = Helper.DateTimeToStamp(datetime);
                userChatMsg.chatMessageId = Guid.NewGuid().ToString();
                userChatMsg.senderId = EncryptUtils.EncryptObject(fromUser.Id);
                userChatMsg.senderName = fromUser.FullName;
                userChatMsg.timestamp = dateStamp;

                chatClient.receiveChatMessage(fromUser.Id.ToString(), userChatMsg, serverChatRoom.ToClientChatRoom(), _UserOnlines.ContainsKey(toChatUser.Id));
                chatClient.receiveChatMessage(toChatUser.Id.ToString(), userChatMsg, serverChatRoom.ToClientChatRoom(), _UserOnlines.ContainsKey(fromUser.Id));

                var chatMessage = new ChatMessage
                {
                    RoomId = userChatMsg.chatRoomId,
                    FromUserId = fromUser.Id,
                    ToUserId = toChatUser.Id,
                    Message = userChatMsg.messageText,
                    CreateDate = datetime,
                    CreateDateStamp = dateStamp,
                    IsRead = false,
                    Status = 0
                };

                ChatMessageBO.Insert(chatMessage);
            }
        }

        public void EndChat(int roomId)
        {
            
        }

        public void RequestHistory(int roomId, long lastMessageId)
        {
            
            var fromUser = Helper.CurrentUser;
            if (fromUser == null)
            {
                return;
            }

            ServerChatRoom serverChatRoom;
            _OnlineChatRooms.TryGetValue(roomId, out serverChatRoom);
            if (serverChatRoom != null)
            {
                if (lastMessageId == 0)
                {
                    lastMessageId = Int64.MaxValue;
                }
                var chatMessages = ChatMessageBO.GetHistoriesByTop(roomId, lastMessageId, 10) ?? new List<ChatMessage>();
                chatClient.sendHistoryMessages(chatMessages.Select(c => c.ToUserChatMessage()).ToList(), serverChatRoom.ToClientChatRoom());
            }
        }

        public void RequestOnlineUsers()
        {
            var users = UserBO.GetAll();
            var currentUser = Helper.CurrentUser;
            var list = new List<ClientChatUser>();
            ClientChatUser user = null;
            for (int i = 0; i < users.Count; i++)
            {
                if (currentUser.Id == users[i].Id)
                {
                    continue;
                }
                user = users[i].ToClientChatUser();
                user.IsOnline = _UserOnlines.ContainsKey(users[i].Id);
                if (string.IsNullOrEmpty(user.Avatar))
                {
                    user.Avatar = users[i].Gender ? "/Content/img/avatar5.png" : "/Content/img/avatar3.png";
                }
                user.OnlineStatus = user.IsOnline ? string.Empty : "offline-status";
                list.Add(user);
            }

            chatClient.updateOnlineUsers(list);
        }

        public void UpdateReadMessage(int roomId)
        {
            var fromUser = Helper.CurrentUser;
            if (fromUser == null)
            {
                return;
            }

            ChatMessageBO.UpdateIsRead(roomId, fromUser.Id);
            chatClient.updateReadMessage(roomId, fromUser.Id);
        }

        #endregion

        #region private method

        private void AddUserOnline(User chatUser)
        {
            var userId = chatUser.Id;
            
            lock (_UserOnlines)
            {
                if (!_UserOnlines.ContainsKey(userId))
                {
                    _UserOnlines.TryAdd(userId, chatUser);
                }
                else
                {
                    _UserOnlines[userId].Count++;
                }
            }
        }

        private bool RemoveUserOnline(int userId)
        {
            if (_UserOnlines.ContainsKey(userId))
            {
                var chatUser = _UserOnlines[userId];
                lock (_UserOnlines)
                {
                    if (chatUser.Count == 1)
                    {
                        return _UserOnlines.TryRemove(userId, out chatUser);
                    }
                    else
                    {
                        chatUser.Count--;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}