using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    public class ChatController : BaseController
    {
        //
        // GET: /Chat/

        public JsonResult GetUnreadChatMessage()
        {
            var unreadRooms = new List<dynamic>();

            // Key: Time stamp, value: RoomId
            var dicRooms = new Dictionary<long, int>();

            // Key: RoomId, value: message and room
            var dicUnreadRooms = new Dictionary<int, dynamic>();

            var user = CurrentUser;
            if (user != null)
            {
                var rooms = ChatRoomBO.GetByUserId(user.Id);
                if (rooms != null && rooms.Count > 0)
                {
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        var lastChatMessage = ChatMessageBO.GetLastestMessageByRoomId(rooms[i].Id);
                        if (lastChatMessage != null &&  lastChatMessage.ToUserId == user.Id &&!lastChatMessage.IsRead)
                        {
                            var otherUser = UserBO.GetById(lastChatMessage.FromUserId);
                            dicUnreadRooms.Add(rooms[i].Id, new
                                {
                                    User = otherUser.ToClientChatUser(),
                                    ChatMessage = lastChatMessage.ToUserChatMessage()
                                });
                            dicRooms.Add(lastChatMessage.CreateDateStamp, rooms[i].Id);
                        }
                    }
                }
            }

            if (dicRooms.Count > 0)
            {
                var keys = dicRooms.Keys.ToList().OrderByDescending(k => k).ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    unreadRooms.Add(dicUnreadRooms[dicRooms[keys[i]]]);
                }
            }
            
            return Json(unreadRooms, JsonRequestBehavior.AllowGet);
        }

    }
}
