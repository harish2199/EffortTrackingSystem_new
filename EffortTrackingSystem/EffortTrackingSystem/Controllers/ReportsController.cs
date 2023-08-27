using CommonDataAccess;
using CommonDataAccess.Models;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    [CommonAuthorize]
    public class ReportsController : BaseController
    {
        public ActionResult Index(int? year, int? month, int? day, int? user, int page = 1)
        {
            try
            {
                // Preserve selected options during pagination
                ViewBag.Year = year;
                ViewBag.Month = month;
                ViewBag.Day = day;
                ViewBag.User = user;

                bool isAdmin = Session["Role"].ToString().ToLower() == "admin";
                int userId = (int)Session["Id"];

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

                return View(efforts);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in the ReportsController: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while generating the report.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
