using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Entity;

namespace TaskManager.Models
{
    public class UserModel
    {
        
        public int Id { set; get; }
        [DisplayName("User Name")]
        public string UserName { set; get; }
        [DisplayName("Password")]
        public string Password { set; get; }
        [DisplayName("First Name")]
        public string FirstName { set; get; }
        [DisplayName("Last Name")]
        public string LastName { set; get; }
        [DisplayName("Address")]
        public string Address { set; get; }
        [DisplayName("Date Of Birth")]
        public string DateOfBirth { set; get; }
        [DisplayName("Gender")]
        public bool Gender { set; get; }
        [DisplayName("Department")]
        public int DepartmentId { set; get; }
        [DisplayName("Email")]
        public string Email { set; get; }
        [DisplayName("Position")]
        public string Mission { set; get; }
        [DisplayName("Avatar")]
        public string Avatar { set; get; }
        [DisplayName("Status")]
        public bool IsActive { set; get; }
        [DisplayName("Role")]
        public bool IsAdmin { set; get; }
        [DisplayName("Manager")]
        public bool IsManager { set; get; }
        
        //public Department Department { set; get; }
    }

    public class UserDisplayModel : UserModel
    {
        public string DepartmentName { set; get; }
        public string CreatedDate { get; set; }
    }

    public class LoginModel
    {
        [DisplayName("UserName")]
        public string UserName { set; get; }

        [DisplayName("Password")]
        public string Password { set; get; }
    }

    public class ChangePassword
    {
        [DisplayName("Old Password")]
        public string OldPassword { set; get; }

        [DisplayName("New Password")]
        public string NewPassword { set; get; }

        [DisplayName("Confirm Password")]
        public string ConfirmPassword { set; get; }
    }

    public class UserSearchModel
    {
        [DisplayName("User Name")]
        public string UserName { set; get; }

        [DisplayName("Full Name")]
        public string FullName { set; get; }

        [DisplayName("Department")]
        public int DepartmentId { set; get; }

        public List<UserDisplayModel> Users { set; get; }

        public UserSearchModel()
        {
            Users = new List<UserDisplayModel>();
        }

    }
}