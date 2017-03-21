using System.Globalization;
using Business;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    public class TaskController : BaseController
    {
        //
        // GET: /Task/

        public ActionResult Index(TasksSearchModel searchModel)
        {
            var listTasks = new List<TasksDisplayModel>();
            if (searchModel == null)
            {
                searchModel = new TasksSearchModel
                    {
                        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(Helper.FormatDate),
                        EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).ToString(Helper.FormatDate),
                    };
            }
            DateTime sdate;
            DateTime edate;
            DateTime.TryParseExact(searchModel.StartDate, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out sdate);
            DateTime.TryParseExact(searchModel.EndDate, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out edate);

            var departments = new List<Department> {new Department {UserId = 0, Name = "--- All ---"}};
            departments.AddRange(DepartmentBO.GetAll());
            ViewBag.Departments = departments;

            var projects = new List<Project> { new Project { Id = 0, Name = "--- All ---" } };
            projects.AddRange(ProjectBO.GetAll());
            ViewBag.Projects = projects;

            var tasks = TaskBO.Search(searchModel.Name, searchModel.Leader, sdate, edate);
            foreach (var item in tasks)
            {
                var department = DepartmentBO.GetById(item.Leader);
                var project = ProjectBO.GetById(item.ProjectId);
                var model = new TasksDisplayModel
                {
                    Id = item.Id,
                    ProjectId = item.ProjectId,
                    ProjectName = project.Name,
                    Name=item.Name,
                    Description=item.Description,
                    Leader = item.Leader,
                    LeaderName=department.Name,
                    Priority=item.Priority,
                    StartDate=item.StartDate,
                    EndDate=item.EndDate,
                    User=UserBO.GetById(item.Leader),
                    CreatedDate=item.CreateDate==null ? string.Empty : ((DateTime)item.CreateDate).ToString(Helper.FormatDate),
                };

                if (CurrentUser.IsManager || CurrentUser.DepartmentLeader == item.Leader)
                {
                    listTasks.Add(model);
                }
            }
            searchModel.Tasks = listTasks;
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            PreparingData();
            return View();
        }

        [HttpPost]
        public ActionResult Create(TasksModel model)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                var task = new Task();
                task.ProjectId = model.ProjectId;
                task.Leader = model.Leader;
                task.Name = model.Name;
                task.Priority = model.Priority;
                task.Description = model.Description;
                task.CreateBy = CurrentUser.Id;
                task.ModifyBy = CurrentUser.Id;
                task.CreateDate = DateTime.Now;
                task.ModifyDate = DateTime.Now;
                if (!string.IsNullOrEmpty(model.StartAndEndDate) && model.StartAndEndDate.Split('-').Length == 2)
                {
                    var strStart = model.StartAndEndDate.Split('-')[0].Trim();
                    var strEnd = model.StartAndEndDate.Split('-')[1].Trim();
                    DateTime sdate;
                    DateTime edate;
                    if (DateTime.TryParseExact(strStart, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out sdate))
                    {
                        task.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        task.EndDate = edate;
                    }

                    TaskBO.Insert(task);
                    var department = DepartmentBO.GetById(task.Leader);
                    if (department != null && department.UserId > 0)
                    {
                        AlertBO.Insert(task.Name, (int)AlertType.Task, 0, department.UserId);    
                    }
                    
                    //TODO ... Insert document
                }
                return RedirectToAction("Index");
            }

            PreparingData();
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var item = TaskBO.GetById(id);
            TasksModel model;
            if (item != null)
            {
                model = new TasksModel
                    {
                        Id = item.Id,
                        ProjectId=item.ProjectId,
                        Name = item.Name,
                        Description = item.Description,
                        Priority = item.Priority,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        Leader = item.Leader,
                        StartAndEndDate = item.StartDate.ToString(Helper.FormatDate) + " - " + item.EndDate.ToString(Helper.FormatDate),
                        DocumentIds = string.Empty,
                        Documents = new List<Document>()
                    };
            }else
            {
                return RedirectToAction("NotFound", "Home");
            }

            PreparingData();
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(TasksModel model)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (model != null)
            {
                var task = TaskBO.GetById(model.Id);
                if (task == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }
                task.ProjectId = model.ProjectId;
                task.Leader = model.Leader;
                task.Name = model.Name;
                task.Priority = model.Priority;
                task.Description = model.Description;
                task.ModifyBy = CurrentUser.Id;
                task.ModifyDate = DateTime.Now;
                if (!string.IsNullOrEmpty(model.StartAndEndDate) && model.StartAndEndDate.Split('-').Length == 2)
                {
                    var strStart = model.StartAndEndDate.Split('-')[0].Trim();
                    var strEnd = model.StartAndEndDate.Split('-')[1].Trim();
                    DateTime sdate;
                    DateTime edate;
                    if (DateTime.TryParseExact(strStart, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out sdate))
                    {
                        task.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        task.EndDate = edate;
                    }

                    TaskBO.Update(task);
                    //TODO ... Insert document
                }
                return RedirectToAction("Index");
            }
            model = new TasksModel();
            PreparingData();
            return View("Create", model);
        }

        public void PreparingData()
        {
            var listPriority = new List<dynamic>()
                {
                    new
                        {
                            Name = Priority.High.GetDescription(),
                            Value = (byte) Priority.High
                        },
                    new
                    {
                        Name = Priority.Normal.GetDescription(),
                        Value = (byte) Priority.Normal
                    },
                    new
                    {
                        Name = Priority.Low.GetDescription(),
                        Value = (byte) Priority.Low
                    },
                };

            ViewBag.Priorities = listPriority;
            ViewBag.Departments = DepartmentBO.GetAll();
            ViewBag.Projects = ProjectBO.GetAll();
        }

        public ActionResult Delete(int id)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            TaskBO.TaskDelete(id);
            AlertBO.Delete(id, (int)AlertType.Task);
            return RedirectToAction("Index");
        }

    }
}
