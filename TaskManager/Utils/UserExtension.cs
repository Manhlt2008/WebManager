using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using TaskManager.Models;
using TaskManager.Models.ChatModels;

namespace TaskManager.Utils
{
    public static class UserExtension
    {
        public static User ToUser(this UserModel userModel)
        {
            return new User
            {
                
            };
        }

        public static UserModel ToUserModel(this User user)
        {
            return new UserModel
            {
                    
            };
        }

        public static ClientChatUser ToClientChatUser(this User user)
        {
            return  new ClientChatUser
                {
                    UserId =  EncryptUtils.EncryptObject(user.Id),
                    Avatar = user.Avatar,
                    FullName = user.FullName,
                    UserName = user.UserName
                };
        }


    }
}