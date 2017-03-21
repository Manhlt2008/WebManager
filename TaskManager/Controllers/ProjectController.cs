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
    public class ProjectController : BaseController
    {
        //
        // GET: /Project/

        public ActionResult Index(ProjectSearchModel searchModel)
        {
            var listProjects = new List<ProjectDisplayModel>();
            if (searchModel == null)
            {
                searchModel = new ProjectSearchModel
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

            var projects = ProjectBO.Search(searchModel.Name, sdate, edate);
            foreach (var item in projects)
            {
                var project = ProjectBO.GetById(item.Id);
                var model = new ProjectDisplayModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    CreatedDate = item.CreateDate == null ? string.Empty : ((DateTime)item.CreateDate).ToString(Helper.FormatDate),
                };

                if (CurrentUser.IsManager)
                {
                    listProjects.Add(model);
                }
            }
            searchModel.Projects = listProjects;
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProjectModel model)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                var project = new Project();
                project.Name = model.Name;
                project.Description = model.Description;
                project.CreateBy = CurrentUser.Id;
                project.ModifyBy = CurrentUser.Id;
                project.CreateDate = DateTime.Now;
                project.ModifyDate = DateTime.Now;
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
                        project.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        project.EndDate = edate;
                    }

                    ProjectBO.Insert(project);
                    //TODO ... Insert document
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var item = ProjectBO.GetById(id);
            ProjectModel model;
            if (item != null)
            {
                model = new ProjectModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description=item.Description,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    StartAndEndDate = item.StartDate.ToString(Helper.FormatDate) + " - " + item.EndDate.ToString(Helper.FormatDate),
                };
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Edit(ProjectModel model)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (model != null)
            {
                var project = ProjectBO.GetById(model.Id);
                if (project == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }
                project.Name = model.Name;
                project.Description = model.Description;
                project.CreateBy = CurrentUser.Id;
                project.ModifyBy = CurrentUser.Id;
                project.CreateDate = DateTime.Now;
                project.ModifyDate = DateTime.Now;
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
                        project.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDate,
                                       new CultureInfo("en-US"),
                                       DateTimeStyles.None,
                                       out edate))
                    {
                        project.EndDate = edate;
                    }

                    ProjectBO.Update(project);
                    //TODO ... Insert document
                }
                return RedirectToAction("Index");
            }
            model = new ProjectModel();
            return View("Create", model);
        }

        public ActionResult Delete(int id)
        {
            if (!CurrentUser.IsManager)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ProjectBO.ProjectDelete(id);
            return RedirectToAction("Index");
        }
    }
}
