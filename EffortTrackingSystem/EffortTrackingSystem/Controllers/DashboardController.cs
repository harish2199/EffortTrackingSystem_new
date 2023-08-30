using Common.Models;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    [UserAuthorize]
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                ViewBag.LoggedIn = TempData["LoggedIn"] as string;
                int userId = GetCurrentUserId();
                

                List<AssignTask> presentTasks = GetPresentTasksForUser(userId);
                List<Effort> previousEfforts = GetLastWeekApprovedEffortsForUser(userId);

                ViewBag.PreviousEfforts = previousEfforts;
                return View(presentTasks);
            }
            catch (Exception ex)
            {
                HandleDashboardError(ex);
                return RedirectToAction("Error", "Home");
            }
        }

        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }

        private List<AssignTask> GetPresentTasksForUser(int userId)
        {
            DateTime today = DateTime.Now.Date;
            return _assignTaskDataAccess.GetAssignedTasksById(userId)
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .ToList();
        }

        private List<Effort> GetLastWeekApprovedEffortsForUser(int userId)
        {
            DateTime today = DateTime.Now.Date;
            DateTime lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 6);
            DateTime lastWeekEnd = lastWeekStart.AddDays(4);

            return _effortDataAccess.GetEfforts()
                .Where(e => e.AssignTask.User.UserId == userId && e.Status == "Approved" && e.SubmittedDate >= lastWeekStart && e.SubmittedDate <= lastWeekEnd)
                .ToList();
        }

        private void HandleDashboardError(Exception ex)
        {
            _log.Error("An error occurred in the Dashboard: " + ex.Message);
            TempData["ErrorMessage"] = "An error occurred while loading the dashboard." + ex;
        }
    }
}
