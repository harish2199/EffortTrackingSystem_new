using CommonDataAccess;
using CommonDataAccess.Models;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    [CommonAuthorize]
    public class ReportsController : BaseController
    {
        public ActionResult Index(int? year, int? month, int? day, int? user, string export, int page = 1)
        {
            try
            {
                ViewBag.SuccessMessage = TempData["ExportMessage"] as string;
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                // Preserve selected options during pagination
                ViewBag.Year = year;
                ViewBag.Month = month;
                ViewBag.Day = day;
                ViewBag.User = user;

                bool isAdmin = IsAdminUser();
                int userId = GetCurrentUserId();

                ViewBag.ShowUserDropdown = isAdmin;
                ViewBag.Users = _userDataAccess.GetAllUsers().ToList();

                List<Effort> efforts = new List<Effort>();
                if (!isAdmin)
                {
                    efforts = _effortDataAccess.GetEffortsByDate(year, month, day)
                        .Where(e => e.AssignTask.UserId == userId)
                        .ToList();
                }
                else if (user.HasValue)
                {
                    efforts = _effortDataAccess.GetEffortsByDate(year, month, day)
                        .Where(e => e.AssignTask.UserId == user)
                        .ToList();
                }
                else
                {
                    efforts = _effortDataAccess.GetEffortsByDate(year, month, day)
                        .ToList();
                }

                /* Pagination */
                int pageSize = 5;
                int total = efforts.Count;
                efforts = efforts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.PageNumber = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

                if (export != null)
                {
                    return ExportToCsv(efforts);
                }

                return View(efforts);
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while generating the report.");
                return RedirectToAction("Error", "Home");
            }
        }


        private ActionResult ExportToCsv(List<Effort> data)
        {
            try
            {
                if (data.Count > 0 && data != null)
                {
                    var csvBuilder = new StringBuilder();
                    csvBuilder.AppendLine("User,Date,Project,Task,Shift,Hours");
                    foreach (var item in data)
                    {
                        var formatUser = item.AssignTask.User.UserName;
                        var formatDate = $"\t{item.SubmittedDate.ToString("yyyy-MM-dd")}";
                        var formatStartTime = item.Shift.StartTime.ToString(@"hh\:mm");
                        var formatEndTime = item.Shift.EndTime.ToString(@"hh\:mm");
                        csvBuilder.AppendLine($"{formatUser},{formatDate},{item.AssignTask.Project.ProjectName},{item.AssignTask.Task.TaskName},{formatStartTime}-{formatEndTime},{item.HoursWorked}");
                    }
                    byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                    TempData["ExportMessage"] = "Exporting to a file was done.";
                    return File(csvBytes, "text/csv", "report.csv");
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot export to file as there are no records.";
                    return RedirectToAction("Index", new { year = Request.QueryString["year"], month = Request.QueryString["month"], day = Request.QueryString["day"], user = Request.QueryString["user"] });
                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while exporting the report to CSV.", ex);
                TempData["ErrorMessage"] = $"An error occurred while exporting the report to CSV. {ex}";
                return RedirectToAction("Error", "Home");
            }
        }
        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }
        private bool IsAdminUser()
        {
            return Session["Role"].ToString().ToLower() == "admin";
        }
        private void HandleError(Exception ex, string errorMessage)
        {
            _log.Error($"{errorMessage} {ex.Message}");
            TempData["ErrorMessage"] = errorMessage;
        }
    }
}
