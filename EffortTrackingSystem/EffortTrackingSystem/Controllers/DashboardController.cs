using Common.Models;
using EffortTrackingSystem.Attributes;
using EffortTrackingSystem.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Controller for the user dashboard.
    /// </summary>
    [MyAuthenticationFilter]
    public class DashboardController : BaseController
    {
        /// <summary>
        /// Displays the user dashboard.
        /// </summary>
        /// <returns>Dashboard view.</returns>
        public ActionResult Index()
        {
            try
            {
                ViewBag.LoggedIn = TempData["LoggedIn"] as string;
                int userId = GetCurrentUserId();

                AssignTask model = _assignTaskDataAccess.GetPresentTaskForUser(userId);
                List<Effort> previousEfforts = GetLastWeekEffortsForUser(userId);
                ViewBag.PreviousEfforts = previousEfforts;

                return View(model);
            }
            catch (Exception ex)
            {
                HandleDashboardError(ex);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Gets the current user's ID from the session.
        /// </summary>
        /// <returns>The current user's ID.</returns>
        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }

        /// <summary>
        /// Retrieves approved efforts from the last week for the user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>List of approved efforts from the last week.</returns>
        private List<Effort> GetLastWeekEffortsForUser(int userId)
        {
            DateTime today = DateTime.Now.Date;
            DateTime lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 6);
            DateTime lastWeekEnd = lastWeekStart.AddDays(4);

            return _effortDataAccess.GetApprovedEffortsOfUser(userId)
                .Where(e => e.SubmittedDate >= lastWeekStart && e.SubmittedDate <= lastWeekEnd)
                .ToList();
        }

        /// <summary>
        /// Handles errors occurring in the dashboard.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        private void HandleDashboardError(Exception ex)
        {
            _log.Error("An error occurred in the Dashboard: " + ex.Message);
            TempData["ErrorMessage"] = "An error occurred while loading the dashboard." + ex;
        }
    }
}
