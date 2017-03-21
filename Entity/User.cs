using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class User : RecordInfo
    {
        public int Id { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string Address { set; get; }
        public DateTime DateOfBirth { set; get; }
        public bool Gender { set; get; }
        public int DepartmentId { set; get; }
        public string Email { get; set; }
        public string Mission { get; set; }
        public string Avatar { set; get; }
        public bool IsActive { set; get; }
        public bool IsAdmin { set; get; }
        public bool IsManager { get; set; }
        public Department Department { set; get; }
        public int Count { set; get; }
        public int DepartmentLeader {get;set;}
        public string FullName
        {
            get 
            {
                var fullName = FirstName + " " + LastName;
                return string.IsNullOrEmpty(fullName.Trim()) ? UserName : fullName.Trim();
            }
        }
    }
}
