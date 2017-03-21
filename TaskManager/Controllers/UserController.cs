using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Business;
using Entity;
using Newtonsoft.Json;
using TaskManager.Hubs;
using TaskManager.Models;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index(UserSearchModel searchModel)
        {
            var listUser = new List<UserDisplayModel>();
            if (searchModel == null)
            {
                searchModel = new UserSearchModel();
            }

            var departments = new List<Department>();
            departments.Add(new Department{Id = 0, Name = "--- All ---"});
            departments.AddRange(DepartmentBO.GetAll());
            ViewBag.Departments = departments;
            var users = UserBO.Search(searchModel.UserName, searchModel.FullName, searchModel.DepartmentId);
            foreach (var item in users)
            {
                var department = DepartmentBO.GetById(item.DepartmentId);
                var model = new UserDisplayModel
                    {
                        Id = item.Id,
                        UserName = item.UserName,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Password = item.Password,
                        Address = item.Address,
                        Avatar = item.Avatar,
                        DateOfBirth = item.DateOfBirth.ToString("dd/MM/yyyy"),
                        DepartmentId = item.DepartmentId,
                        DepartmentName = department == null? string.Empty : department.Name,
                        Gender = item.Gender,
                        Email=item.Email,
                        Mission=item.Mission,
                        IsActive = item.IsActive,
                        IsAdmin = item.IsAdmin,
                        CreatedDate=item.CreateDate==null ? string.Empty : ((DateTime)item.CreateDate).ToString(Helper.FormatDate),
                    };
                listUser.Add(model);
            }

            searchModel.Users = listUser;
            return View(searchModel);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {

            if (CurrentUser != null)
            {
                return Redirect(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                // Pass 123456: dSVrtpZkGVI=
                var user = UserBO.Login(model.UserName, EncryptUtils.Encrypt(model.Password));
                if (user != null)
                {
                    user.Department = DepartmentBO.GetById(user.DepartmentId);
                    FormsAuthentication.SetAuthCookie(JsonConvert.SerializeObject(user, Formatting.None), false);
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "User or password incorrect");
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Department = DepartmentBO.GetAll();
            var model = new UserModel {Gender = true, IsActive = true};
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(UserModel model)
        {
            if (Validate(model))
            {
                model.Avatar = model.Gender ? "/Content/img/avatar5.png" : "/Content/img/avatar3.png";
                DateTime date;
                if(!DateTime.TryParseExact(model.DateOfBirth, "dd/MM/yyyy",
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out date))
                {
                    date = DateTime.Now;
                }
                UserBO.Create(model.UserName, EncryptUtils.Encrypt(model.Password), model.FirstName, model.LastName, model.Address, date, model.Gender, model.DepartmentId,model.Email,model.Mission, model.Avatar, true, false,model.IsManager, DateTime.Now, DateTime.Now, CurrentUser.Id, CurrentUser.Id);
                return RedirectToAction("Index");
            }
            var departments = DepartmentBO.GetAll();
            ViewBag.Department = departments;
            return View(model);
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Department = DepartmentBO.GetAll();
            var user = UserBO.GetById(id);
            
            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = new UserModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = EncryptUtils.Decrypt(user.Password),
                Address = user.Address,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth.ToString("dd/MM/yyyy"),
                DepartmentId = user.DepartmentId,
                Gender = user.Gender,
                Email = user.Email,
                Mission = user.Mission,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin
            };

            return View("Create", model);
        }
        [HttpPost]
        public ActionResult Edit(UserModel model)
        {
            var user = UserBO.GetById(model.Id);
            if (user != null)
            {
                user.Avatar = model.Gender ? "/Content/img/avatar5.png" : "/Content/img/avatar3.png";
                DateTime date;
                if (DateTime.TryParseExact(model.DateOfBirth, "dd/MM/yyyy",
                                           new CultureInfo("en-US"),
                                           DateTimeStyles.None,
                                           out date))
                {
                    user.DateOfBirth = date;
                }

                UserBO.Update(user.Id, user.UserName, user.Password, model.FirstName, model.LastName, model.Address, user.DateOfBirth, model.Gender, model.DepartmentId,model.Email,model.Mission, user.Avatar, model.IsActive, model.IsAdmin,model.IsManager, user.CreateDate, DateTime.Now, user.CreateBy, CurrentUser.Id);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Edit not succeed");
            }
            ViewBag.Department = DepartmentBO.GetAll();
            return View("Create", model);
        }
        public ActionResult Delete(int userId)
        {
            if (UserBO.GetById(userId) != null)
            {
                UserBO.Delete(userId);
                ModelState.AddModelError("", "Delete succeed");
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new ChangePassword());
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid) 
            {
                if (CurrentUser.Password != EncryptUtils.Encrypt(model.OldPassword).Trim())
                {
                    ModelState.AddModelError("", "Current password not correct");
                    return View();
                }
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError("", "You must enter your new password");
                    return View();
                }
                else
                {
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "Confirm password not correct");
                        return View();
                    }
                    else
                    {
                        UserBO.ChangePassword(CurrentUser.Id, EncryptUtils.Encrypt(model.NewPassword));
                        CurrentUser.Password = EncryptUtils.Encrypt(model.NewPassword);
                        FormsAuthentication.SetAuthCookie(JsonConvert.SerializeObject(CurrentUser, Formatting.None), false);
                        return RedirectToAction("MyProfile", "User");
                    }
                }
            }
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            var user = CurrentUser;
            if (user != null)
            {
                if (ChatServer._UserOnlines.ContainsKey(user.Id))
                {
                    User u;
                    ChatServer._UserOnlines.TryRemove(user.Id, out u);
                }
            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public bool Validate(UserModel model)
        {
            var isValid = true;

            if (model.UserName == null || model.UserName.Trim().Length == 0)
            {
                ModelState.AddModelError("", "You must enter your User Name");
                isValid = false;
            }
            if (model.Password == null || model.Password.Trim().Length == 0)
            {
                ModelState.AddModelError("", "You must enter your Password");
                isValid = false;
            }
            return isValid;
        }
    }
}
