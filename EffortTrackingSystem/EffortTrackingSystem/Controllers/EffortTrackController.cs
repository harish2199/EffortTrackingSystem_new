using Common.Models;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Controller for tracking effort, leave, and shift change submissions.
    /// </summary>
    [CommonAuthorize]
    public class EffortTrackController : BaseController
    {
        /// <summary>
        /// Displays the Effort Tracking page.
        /// </summary>
        /// <returns>Effort Tracking view.</returns>
        public ActionResult Index()
        {
            try
            {
                ViewBag.Message = TempData["message"] as string;

                int userId = GetCurrentUserId();
                ViewBag.UserId = userId;

                AssignTask presentTask = _assignTaskDataAccess.GetPresentTaskForUser(userId);
                ViewBag.Shifts = _shiftDataAccess.GetShifts().ToList();

                return View(presentTask);
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while loading the Effort Tracking page.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Submits an effort.
        /// </summary>
        /// <param name="effort">The effort to submit.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult SubmitEffort(Effort effort)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                string message = _effortDataAccess.SubmitEffort(effort, userId);
                if (message.Contains("submitted successfully"))
                {
                    string subject = $"New Effort Submission";
                    string body = $"New Effort submitted by {Session["Name"]}";
                    SendEmail(subject, body);
                }

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while submitting effort.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Submits a leave request.
        /// </summary>
        /// <param name="leave">The leave request.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult SubmitLeave(Leave leave)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                string message = _leaveDataAccess.SubmitLeave(leave);
                if (message.Contains("submitted successfully"))
                {
                    string subject = $"Leave Request";
                    string body = $"Leave requested submitted by {Session["Name"]}";
                    SendEmail(subject, body);
                }

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while submitting leave.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Submits a shift change request.
        /// </summary>
        /// <param name="shiftChange">The shift change request.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult SubmitShiftChange(ShiftChange shiftChange)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                string message = _shiftChangeDataAccess.SubmitShiftChange(shiftChange);
                if (message.Contains("submitted successfully"))
                {
                    string subject = $"Shift Change Submission";
                    string body = $"Shift change requested submitted by {Session["Name"]}";
                    SendEmail(subject, body);
                }

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while submitting shift change.");
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
        /// Handles errors by logging and setting an error message.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="errorMessage">The custom error message.</param>
        private void HandleError(Exception ex, string errorMessage)
        {
            _log.Error($"{errorMessage} {ex.Message}");
            TempData["ErrorMessage"] = errorMessage + ex;
        }
    }
}
