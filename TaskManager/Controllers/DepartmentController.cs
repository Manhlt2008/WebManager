using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Business;
using Entity;
using Newtonsoft.Json;
using TaskManager.Models;
using TaskManager.Utils;
namespace TaskManager.Controllers
{
    public class DepartmentController : BaseController
    {
        //
        // GET: /Department/

        public ActionResult Index()
        {
            var listDepartmentsModel = new List<DepartmentModel>();
            var listDepartments = DepartmentBO.GetAll();
            foreach(var item in listDepartments)
            {
                var model = new DepartmentModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        UserId = item.UserId,
                        User = UserBO.GetById(item.UserId),
                        CreatedDate=item.CreateDate==null ? string.Empty: ((DateTime)item.CreateDate).ToString(Helper.FormatDate),
                    };
                listDepartmentsModel.Add(model);
            }
            return View(listDepartmentsModel);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.CurrentUser = CurrentUser;
            var listUsers=UserBO.GetAll();
            ViewBag.ListUsers = listUsers;
            return View();
        }

        [HttpPost]
        public ActionResult Create(DepartmentModel model)
        {
            if (model != null && Validate(model))
            {
                if(Validate(model))
                {
                    DepartmentBO.Create(model.Name, model.UserId, DateTime.Now, DateTime.Now, CurrentUser.Id, CurrentUser.Id);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Create deparment not succeed");
                return RedirectToAction("Create");
            }
            return View(model);
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.ListUsers = UserBO.GetAll();
            var department = DepartmentBO.GetById(id);
            if (department == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var model = new DepartmentModel()
            {
                Id=department.Id,
                Name=department.Name,
                UserId=department.UserId,
            };

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(DepartmentModel model)
        {
            if (DepartmentBO.GetById(model.Id)!=null)
            {
                if(Validate(model))
                {
                    DepartmentBO.Update(model.Id, model.Name, model.UserId, DateTime.Now, DateTime.Now, CurrentUser.Id, CurrentUser.Id);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.ListUsers = UserBO.GetAll();
            return View(model);
        }

        
        public ActionResult Delete(int departmentId)
        {
            if (DepartmentBO.GetById(departmentId) != null)
            {
                DepartmentBO.Delete(departmentId);
                return RedirectToAction("Index");
            }
            else
            {
                //ModelState.AddModelError("", "Có lỗi trong quá trình xóa");
                return RedirectToAction("NotFound", "Home");
            }
            return View();
        }
        public bool Validate(DepartmentModel model)
        {
            var isValid = true;

            if (model.Name == null || model.Name.Trim().Length == 0)
            {
                ModelState.AddModelError("", "You must enter department name");
                isValid = false;
            }
            return isValid;
        }
    }
}
