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
    [UserAuthorize]
    public class EffortTrackController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                ViewBag.Message = TempData["message"] as string;

                int userId = GetCurrentUserId();
                ViewBag.UserId = userId;
                AssignTask presentTask = GetPresentTasksForUser(userId);
                ViewBag.Shifts = _shiftDataAccess.GetShifts().ToList();
                return View(presentTask);
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while loading the Effort Tracking page.");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult SubmitEffort(Effort effort)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                string message = _effortDataAccess.SubmitEffort(effort);
                if (message.Contains("submitted successfully"))
                {
                    string subject = $"New Effort Submission";
                    String body = $"New Effort submitted by {Session["Name"]}";
                    SendEmailTo(subject, body);
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
                    String body = $"Leave requested submitted by {Session["Name"]}";
                    SendEmailTo(subject, body);
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
                    String body = $"Shift changed requested submitted by {Session["Name"]}";
                    SendEmailTo(subject, body);
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


        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }
        private AssignTask GetPresentTasksForUser(int userId)
        {
            DateTime today = DateTime.Now.Date;
            return _assignTaskDataAccess.GetAssignedTasks(userId)
                .Where(a => a.StartDate <= today && a.EndDate >= today)
                .FirstOrDefault();
        }
        private void HandleError(Exception ex, string errorMessage)
        {
            _log.Error($"{errorMessage} {ex.Message}");
            TempData["ErrorMessage"] = errorMessage;
        }
    }
}
