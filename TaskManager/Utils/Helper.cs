using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using Entity;
using Newtonsoft.Json;
using TaskManager.Models;
using Business;

namespace TaskManager.Utils
{
    public static class Helper
    {
        public static User CurrentUser
        {
            get
            {
                if (HttpContext.Current == null) return null;
                var user = HttpContext.Current.User;
                if (user != null && !string.IsNullOrWhiteSpace(user.Identity.Name))
                {
                    var u=JsonConvert.DeserializeObject<User>(user.Identity.Name);
                    if (u != null)
                    {
                        u = UserBO.GetById(u.Id);
                        if (u!= null)
                        {
                            u.Department = DepartmentBO.GetById(u.DepartmentId);
                            var check = DepartmentBO.CheckExitUserId(u.Id);
                            u.DepartmentLeader = check == null ? 0 : check.Id;    
                        }
                        
                    }
                    return u;
                }
                return null;
            }
        }
        public static string FormatDate = "MM/dd/yyyy";
        public static string FormatDateTime = "MM/dd/yyyy h:mm tt";
        public static long DateTimeToStamp(DateTime dateTime)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (long)span.TotalMilliseconds;
        }

        public static DateTime StampToDateTime(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.ToLocalTime().AddMilliseconds(timestamp);
        }

        public static bool TryParse<T>(string input, out T type)
        {
            bool result;
            try
            {
                type = (T)Convert.ChangeType(input, typeof(T));
                result = true;
            }
            catch
            {
                type = default(T);
                result = false;
            }

            return result;
        }

        public static T ConvertTo<T>(string input)
        {
            try
            {
                return (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static void Exchange<T>(ref T t1, ref T t2)
        {
            T tg = t1;
            t1 = t2;
            t2 = tg;
        }

        public static T GetConfig<T>(string key) where T : new()
        {
            try
            {
                return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
            }
            catch
            {
            }

            return default(T);
        }

        public static void Sort(ref int fromUserId, ref int toUserId)
        {
            if (fromUserId < toUserId)
            {
                var tempUserId = toUserId;
                toUserId = fromUserId;
                fromUserId = tempUserId;
            }
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}