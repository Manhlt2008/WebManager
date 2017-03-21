using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Business;
using Entity;
using TaskManager.Models;
using TaskManager.Utils;

namespace TaskManager.Controllers
{
    public class AppointmentController : BaseController
    {
        //
        // GET: /Appointment/

        public ActionResult Index()
        {
            var user = CurrentUser;
            var appointments = AppointmentBO.GetByUserId(user.Id);
            if (appointments != null && appointments.Count > 0)
            {
            }
            return View(appointments);
        }

        public JsonResult GetAppointment()
        {
            var user = CurrentUser;
            var list = new List<dynamic>();
            var appointments = AppointmentBO.GetByUserId(user.Id);
            if (appointments != null && appointments.Count > 0)
            {
                for (int i = 0; i < appointments.Count; i++)
                {
                    list.Add(new
                    {
                        Id = appointments[i].Id,
                        Title = appointments[i].Title,
                        StartDate = appointments[i].StartDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = appointments[i].EndDate.ToString("yyyy-MM-dd HH:mm"),
                    });
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            var model = new AppointmentModel
                {
                    StartAndEndDate = date.ToString(Helper.FormatDateTime) + " - " + date.AddMinutes(30).ToString(Helper.FormatDateTime)
                };
            PreparingData();
            return View(model);
        }

        [HttpPost]
        public JsonResult Create(AppointmentModel model)
        {
            ResponseAppointment response;
            
            if (Validate(model))
            {
                model.Attendees = new List<int>();
                if (!string.IsNullOrEmpty(model.AttendessStr))
                {
                    var split = model.AttendessStr.Split(',');
                    if (split.Length > 0)
                    {
                        for (int i = 0; i < split.Length; i++)
                        {
                            try
                            {
                                model.Attendees.Add(int.Parse(split[i]));
                                AlertBO.Insert(model.Title, (int)AlertType.Event, 0, int.Parse(split[i]));
                            }
                            catch (Exception)
                            {
                                
                            }
                        }    
                    }
                    
                }
                var appointment = new Appointment
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        CreateBy = CurrentUser.Id,
                        CreateDate = DateTime.Now,
                        ModifyBy = CurrentUser.Id,
                        ModifyDate = DateTime.Now,
                        Attendees =
                            model.Attendees != null
                                ? model.Attendees.Select(u => new User {Id = u}).ToList()
                                : new List<User>()
                    };
                var strStart = model.StartAndEndDate.Split('-')[0].Trim();
                var strEnd = model.StartAndEndDate.Split('-')[1].Trim();
                DateTime sdate;
                DateTime edate;
                if (DateTime.TryParseExact(strStart, Helper.FormatDateTime,
                                           new CultureInfo("en-US"),
                                           DateTimeStyles.None,
                                           out sdate))
                {
                    appointment.StartDate = sdate;
                }

                if (DateTime.TryParseExact(strEnd, Helper.FormatDateTime,
                                           new CultureInfo("en-US"),
                                           DateTimeStyles.None,
                                           out edate))
                {
                    appointment.EndDate = edate;
                }

                AppointmentBO.Insert(appointment);
                response = new ResponseAppointment
                {
                    Result = 1,
                    Appointment = new
                    {
                        Id = appointment.Id,
                        Title = appointment.Title,
                        StartDate = appointment.StartDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = appointment.EndDate.ToString("yyyy-MM-dd HH:mm"),
                    }
                };
            }
            else
            {
                response = new ResponseAppointment
                {
                    Result = 0,
                    ErrorMsg = string.Join("\n", (from state in ModelState.Values
                                                from error in state.Errors
                                                select error.ErrorMessage).ToList()) 
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var appointment = AppointmentBO.GetById(id);
            if (appointment == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            appointment.Attendees = AppointmentBO.GetUsers(appointment.Id);

            var model = new AppointmentModel
                {
                    Id = appointment.Id,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    StartAndEndDate = appointment.StartDate.ToString(Helper.FormatDateTime) + " - " + appointment.EndDate.ToString(Helper.FormatDateTime),
                    Attendees = appointment.Attendees != null ? appointment.Attendees.Select(u=>u.Id).ToList() : new List<int>()
                };

            ViewBag.Count = AppointmentBO.CountCommentAccepted(model.Id);

            ViewBag.DisplayReject = CurrentUser.Id != appointment.CreateBy &&
                                  AppointmentBO.CommentCheckExisted(model.Id, CurrentUser.Id) == null;

            ViewBag.DisplaySave = CurrentUser.Id == appointment.CreateBy;

            PreparingData();
            return View("Create", model);
        }

        [HttpPost]
        public JsonResult Edit(AppointmentModel model)
        {
            ResponseAppointment response;
            if (Validate(model))
            {
                model.Attendees = new List<int>();
                if (!string.IsNullOrEmpty(model.AttendessStr))
                {
                    var split = model.AttendessStr.Split(',');
                    if (split.Length > 0)
                    {
                        for (int i = 0; i < split.Length; i++)
                        {
                            try
                            {
                                model.Attendees.Add(int.Parse(split[i]));
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                }
                var appointment = AppointmentBO.GetById(model.Id);
                if (appointment == null)
                {
                    response = new ResponseAppointment
                        {
                            Result = 0,
                            ErrorMsg = "Not existed"
                        };
                }
                else
                {
                    appointment.Id = model.Id;
                    appointment.Title = model.Title;
                    appointment.Description = model.Description;
                    appointment.ModifyBy = CurrentUser.Id;
                    appointment.ModifyDate = DateTime.Now;
                    appointment.Attendees = model.Attendees != null
                                                ? model.Attendees.Select(u => new User {Id = u}).ToList()
                                                : new List<User>();
                    var strStart = model.StartAndEndDate.Split('-')[0].Trim();
                    var strEnd = model.StartAndEndDate.Split('-')[1].Trim();
                    DateTime sdate;
                    DateTime edate;
                    if (DateTime.TryParseExact(strStart, Helper.FormatDateTime,
                                               new CultureInfo("en-US"),
                                               DateTimeStyles.None,
                                               out sdate))
                    {
                        appointment.StartDate = sdate;
                    }

                    if (DateTime.TryParseExact(strEnd, Helper.FormatDateTime,
                                               new CultureInfo("en-US"),
                                               DateTimeStyles.None,
                                               out edate))
                    {
                        appointment.EndDate = edate;
                    }
                    AppointmentBO.Update(appointment);
                    response = new ResponseAppointment
                    {
                        Result = 1,
                        Appointment = new
                            {
                                Id = appointment.Id,
                                Title = appointment.Title,
                                StartDate = appointment.StartDate.ToString("yyyy-MM-dd HH:mm"),
                                EndDate = appointment.EndDate.ToString("yyyy-MM-dd HH:mm"),
                            }
                    };
                }

            }
            else
            {
                response = new ResponseAppointment
                {
                    Result = 0,
                    ErrorMsg = string.Join("\n", (from state in ModelState.Values
                                                  from error in state.Errors
                                                  select error.ErrorMessage).ToList()) 
                };
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public void PreparingData()
        {
            ViewBag.Users = UserBO.GetAll();
        }

        public bool Validate(AppointmentModel model)
        {
            var isValid = true;

            if (model.Title == null || model.Title.Trim().Length == 0)
            {
                ModelState.AddModelError("","Title is not empty.");
                isValid = false;
            }

            if (string.IsNullOrEmpty(model.StartAndEndDate) || model.StartAndEndDate.Split('-').Length != 2)
            {
                ModelState.AddModelError("", "Start date and end date invalid.");
                isValid = false;
            }
            else
            {
                var strStart = model.StartAndEndDate.Split('-')[0].Trim();
                var strEnd = model.StartAndEndDate.Split('-')[1].Trim();
                DateTime sdate;
                DateTime edate;
                if (!DateTime.TryParseExact(strStart, Helper.FormatDateTime,
                                   new CultureInfo("en-US"),
                                   DateTimeStyles.None,
                                   out sdate) || !DateTime.TryParseExact(strEnd, Helper.FormatDateTime,
                                   new CultureInfo("en-US"),
                                   DateTimeStyles.None,
                                   out edate))
                {
                    ModelState.AddModelError("", "Start date and end date invalid.");
                    isValid = false;
                }
            }

            return isValid;
        }

        public JsonResult Delete(int id)
        {
            AppointmentBO.Delete(id);
            AlertBO.Delete(id, (int)AlertType.Event);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfComment(int id)
        {
            var listcomments = AppointmentBO.GetCommentByAppointmentId(id);
            if (listcomments != null && listcomments.Count > 0)
            {
                for (int i = 0; i < listcomments.Count; i++)
                {
                    listcomments[i].User = UserBO.GetById(listcomments[i].CreateBy);
                }
            }else
            {
                listcomments = new List<AppointmentComment>();
            }
            return View(listcomments);
        }

        [HttpGet]
        public ActionResult Reject(int id)
        {
            var app = AppointmentBO.GetById(id);
            ViewBag.Appointment = app;
            return View();
        }

        [HttpPost]
        public JsonResult Reject(AppointmentComment comment)
        {
            if (comment != null)
            {
                comment.CommentType = CommentType.Rejected;
                comment.CreateBy = CurrentUser.Id;
                comment.CreateDate = DateTime.Now;
                comment.ModifyBy = CurrentUser.Id;
                comment.ModifyDate = DateTime.Now;
                AppointmentBO.AppointmentCommentInsert(comment);    
            }

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Accept(int id)
        {
            AppointmentBO.AppointmentCommentInsert(new AppointmentComment
                {
                    CommentType = (int)CommentType.Accepted,
                    Comments = "Accepted",
                    CreateBy = CurrentUser.Id,
                    CreateDate = DateTime.Now,
                    ModifyBy = CurrentUser.Id,
                    ModifyDate = DateTime.Now,
                    AppointmentId = id
                });
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public class ResponseAppointment
        {
            // 1: success, 0: Error
            public int Result { set; get; }

            public dynamic Appointment { set; get; }

            public string ErrorMsg { set; get; }
        }

    }
}
