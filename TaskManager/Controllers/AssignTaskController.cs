using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Entity;
using TaskManager.Models;
using TaskManager.Utils;
using System.Globalization;

namespace TaskManager.Controllers
{
    public class AssignTaskController : BaseController
    {
        //
        // GET: /AssignTask/

        public ActionResult Index(int id)
        {
            var task = TaskBO.GetById(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var assignTasks = TaskBO.AssignTaskGetByTaskId(id);

            if (assignTasks != null && assignTasks.Count > 0)
            {
                for (int i = 0; i < assignTasks.Count; i++)
                {
                    assignTasks[i].User = UserBO.GetById(assignTasks[i].UserId);
                }
            }

            ViewBag.Task = task;

            return View(assignTasks);
        }
        public ActionResult MyTask(AssignTasksSearchModel searchmodel)
        {
            var listAssignTasks = new List<AssignTask>();
            if (searchmodel == null)
            {
                searchmodel = new AssignTasksSearchModel();
            }
            DateTime sdate;
            DateTime edate;
            DateTime.TryParseExact(searchmodel.StartDate, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out sdate);
            DateTime.TryParseExact(searchmodel.EndDate, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out edate);
            var assigntasks = TaskBO.AssignTaskSearch(searchmodel.Requirement, CurrentUser.Id, sdate, edate);
            if (assigntasks != null)
            {
                foreach (var item in assigntasks)
                {
                    item.Task = TaskBO.GetById(item.TaskId);
                    
                }
                listAssignTasks=assigntasks;
            }
            searchmodel.AssignTask = listAssignTasks;
            return View(searchmodel);
        }
        [HttpGet]
        public ActionResult Create(int id)
        {
            var task = TaskBO.GetById(id);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var assignTask = new AssignTask
                {
                    TaskId = id
                };
            ViewBag.Task = task;
            ViewBag.Users = UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader);
            return View(assignTask);
        }

        [HttpPost]
        public ActionResult Create(AssignTask assignTask)
        {
            var task = TaskBO.GetById(assignTask.TaskId);
            if (task == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            
                assignTask.CreateBy = CurrentUser.Id;
                assignTask.CreateDate = DateTime.Now;
                assignTask.ModifyBy = CurrentUser.Id;
                assignTask.ModifyDate = DateTime.Now;

                if (!string.IsNullOrEmpty(assignTask.StartAndEndDate) && assignTask.StartAndEndDate.Split('-').Length == 2)
                {
                    var strStart = assignTask.StartAndEndDate.Split('-')[0].Trim();
                    var strEnd = assignTask.StartAndEndDate.Split('-')[1].Trim();
                    DateTime sdate;
                    DateTime edate;
                    if (DateTime.TryParseExact(strStart, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out sdate))
                    {
                        assignTask.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        assignTask.EndDate = edate;
                    }

                    TaskBO.AssignTaskInsert(assignTask);
                    AlertBO.Insert(assignTask.Requirement, (int)AlertType.AssignTask, 0, assignTask.UserId);
                }
                return RedirectToAction("Index" , new {id = assignTask.TaskId});
            

            ViewBag.Task = task;
            ViewBag.Users = UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader);
            return View(assignTask);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var assignTask = TaskBO.AssignTaskGetById(id);
            //AssignTask model;
            if (assignTask != null)
            {

                assignTask.StartAndEndDate = assignTask.StartDate.ToString(Helper.FormatDate) + " - " + assignTask.EndDate.ToString(Helper.FormatDate);
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.Task = TaskBO.GetById(assignTask.TaskId);
            ViewBag.Users = UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader);
            return View("Create", assignTask);
        }

        [HttpPost]
        public ActionResult Edit(AssignTask assignTask)
        {
            var assignTaskSave = TaskBO.AssignTaskGetById(assignTask.Id);

            if (assignTaskSave == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (assignTaskSave.UserId > 0)
            {
                assignTaskSave.UserId = assignTask.UserId;
                assignTaskSave.Requirement = assignTask.Requirement;
                assignTaskSave.ModifyBy = CurrentUser.Id;
                assignTaskSave.ModifyDate = DateTime.Now;

                if (!string.IsNullOrEmpty(assignTask.StartAndEndDate) && assignTask.StartAndEndDate.Split('-').Length == 2)
                {
                    var strStart = assignTask.StartAndEndDate.Split('-')[0].Trim();
                    var strEnd = assignTask.StartAndEndDate.Split('-')[1].Trim();
                    DateTime sdate;
                    DateTime edate;
                    if (DateTime.TryParseExact(strStart, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out sdate))
                    {
                        assignTask.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        assignTask.EndDate = edate;
                    }

                    TaskBO.AssignTaskUpdate(assignTask);
                }
                return RedirectToAction("Index", new { id = assignTaskSave.TaskId });
            }

            ViewBag.Task = TaskBO.GetById(assignTask.TaskId);
            ViewBag.Users = UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader);
            return View("Create", assignTask);
        }

        public ActionResult Delete(int id)
        {
            var assignTask = TaskBO.AssignTaskGetById(id);

            if (assignTask == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            TaskBO.AssignTaskDelete(id);
            AlertBO.Delete(id, (int)AlertType.AssignTask);
            return RedirectToAction("Index", new {id = assignTask.TaskId});
        }

        [HttpGet]
        public ActionResult UpdateEmployee(int id)
        {
            var subtask = TaskBO.AssignTaskGetById(id);
            if (subtask == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.SubTask = subtask;
            ViewBag.Task = TaskBO.GetById(subtask.TaskId);
            ViewBag.Users = UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader);
            if (subtask.UserId <= 0 && ViewBag.Users != null && ViewBag.Users.Count > 0)
            {
                subtask.UserId = ViewBag.Users[0].Id;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateEmployee(int subtaskId,int userId)
        {
            var subtask = TaskBO.AssignTaskGetById(subtaskId);
            if (subtask == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            subtask.UserId = userId;
            TaskBO.AssignTaskUpdate(subtask);
            return RedirectToAction("Index", new { id = subtaskId });   
        } 
    }
}
