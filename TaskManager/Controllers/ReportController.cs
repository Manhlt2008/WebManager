using Business;
using Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    public class ReportController : BaseController
    {
        //
        // GET: /Report/

        public ActionResult Index(string date)
        {
            var lstreport=new List<Report>();
            DateTime sdate;
            if (!DateTime.TryParseExact(date, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out sdate))
            {
                sdate = DateTime.Now;
            }
            var reports=TaskBO.ReportByUserReportAndReportDate(CurrentUser.Id,sdate.ToString("dd/MM/yyyy"));
            if (reports != null && reports.Count > 0)
            {
                foreach (var item in reports)
                {
                    item.AssignTask = TaskBO.AssignTaskGetById(item.AssignTaskId);
                }
            }
            ViewBag.Date = sdate;
            return View (reports ?? new List<Report>());
        }
        public ActionResult OverDues(string date)
        {
            DateTime sdate;
            if (!DateTime.TryParseExact(date, Helper.FormatDate,
                               new CultureInfo("en-US"),
                               DateTimeStyles.None,
                               out sdate))
            {
                sdate = DateTime.Now;
            }
            var users = TaskBO.ReportsOverDues(CurrentUser.DepartmentLeader, sdate.ToString("dd/MM/yyyy"));
            ViewBag.Date = sdate;
            return View(users ?? new List<User>());
        }

        [HttpGet]
        public ActionResult Create(int assignTaskId)
        {
            var assignTask = TaskBO.AssignTaskGetById(assignTaskId);
            if (assignTask == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var report = new Report
                {
                    AssignTaskId = assignTaskId
                };
            PrepareData();
            return View(report);
        }

        [HttpPost]
        public ActionResult Create(Report model)
        {
            if (Validate(model))
            {
                var assignTask = TaskBO.AssignTaskGetById(model.AssignTaskId);
                var report = new Report
                    {
                        ReportResult = model.ReportResult,
                        AssignTaskId = model.AssignTaskId,
                        CompletedPercent = model.CompletedPercent,
                        NextTask = model.NextTask,
                        CreateDate = DateTime.Now,
                        ReportDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day),
                        CreateBy = CurrentUser.Id,
                        ModifyBy = CurrentUser.Id,
                        ModifyDate = DateTime.Now,
                        TaskId = assignTask != null ?assignTask.TaskId : 0,
                        UserReport = CurrentUser.Id,
                        CommentDate = DateTime.Now,
                        ReportType=model.ReportType
                    };
                TaskBO.ReportInsert(report);
                AlertBO.Insert(report.ReportResult, (int)AlertType.Report, 0, assignTask.CreateBy);
                TaskBO.AssignTaskUpdateCompleted(model.AssignTaskId, model.CompletedPercent);
                return RedirectToAction("Index");
            }
            PrepareData();
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var report = TaskBO.ReportGetById(id);
            if (report == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            PrepareData();
            return View("Create",report);
        }

        [HttpPost]
        public ActionResult Edit(Report model)
        {
            var report = TaskBO.ReportGetById(model.Id);
            if (report == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (Validate(model))
            {
                report.ReportResult = model.ReportResult;
                report.CompletedPercent = model.CompletedPercent;
                report.NextTask = model.NextTask;
                report.ModifyBy = CurrentUser.Id;
                report.ModifyDate = DateTime.Now;
                report.ReportType = model.ReportType;


                TaskBO.ReportUpdate(report);
                TaskBO.AssignTaskUpdateCompleted(report.AssignTaskId, model.CompletedPercent);
                return RedirectToAction("Index");
            }
            PrepareData();
            return View("Create",model);
        }

        public ActionResult Delete(int id)
        {
            TaskBO.ReportDelete(id);
            AlertBO.Delete(id, (int)AlertType.Comment);
            AlertBO.Delete(id, (int)AlertType.Report);
            return RedirectToAction("Index");
        }

        private bool Validate(Report model)
        {
            var check = true;

            if (string.IsNullOrEmpty(model.ReportResult))
            {
                ModelState.AddModelError("", "Report result is empty.");
                check = false;
            }

            return check;
        }

        public ActionResult ListReport(ReportSearchModel searchModel)
        {
            if (searchModel == null)
            {
                searchModel = new ReportSearchModel();
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

            var reports = TaskBO.ReportSearch(searchModel.UserId, sdate, edate);
            
            var user = new List<User>
                {
                    new User {UserName = "-- All --"}
                };
            user.AddRange(UserBO.GetByDepartmentId(CurrentUser.DepartmentLeader));
            ViewBag.Users = user;
            var temReport = new List<Report>();
            if (reports != null && reports.Count > 0)
            {
                for (int i = 0; i < reports.Count; i++)
                {
                    reports[i].AssignTask = TaskBO.AssignTaskGetById(reports[i].AssignTaskId);
                    reports[i].Task = TaskBO.GetById(reports[i].AssignTask.TaskId);
                    reports[i].User = UserBO.GetById(reports[i].UserReport);

                    
                    if (reports[i].User.DepartmentId == CurrentUser.DepartmentLeader)
                    {
                        temReport.Add(reports[i]);    
                    }
                    
                }    
            }

            searchModel.Reports = temReport;
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult Comment(int id)
        {
            var report = TaskBO.ReportGetById(id);
            if (report == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(report);
        }

        [HttpPost]
        public ActionResult Comment(Report model)
        {
            var report = TaskBO.ReportGetById(model.Id);
            if (report == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!string.IsNullOrEmpty(model.Comment))
            {
                report.Comment = model.Comment;
                report.CommentBy = CurrentUser.Id;
                report.CommentDate = DateTime.Now;
                TaskBO.ReportUpdate(report);
                AlertBO.Insert(report.Comment, (int)AlertType.Comment, 0, report.UserReport);
                return RedirectToAction("ListReport");
            }else
            {
                ModelState.AddModelError("", "Comment is not empty");
            }
            return View(report);
        }

        private void PrepareData() {
            ViewBag.ReportTypes = new List<dynamic>{
                new {
                    Text = "Day",
                    Value = 1
                },
                new {
                    Text = "Month",
                    Value = 2
                },
            };
        }

    }
}
