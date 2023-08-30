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
    /// Controller for administrative tasks and actions.
    /// </summary>
    [CommonAuthorize]
    public class AdminController : BaseController
    {
        /// <summary>
        /// Displays the admin dashboard with relevant information.
        /// </summary>
        /// <returns>Admin dashboard view.</returns>
        public ActionResult Index()
        {
            try
            {
                ViewBag.LoggedIn = TempData["LoggedIn"] as string;
                ViewBag.Message = TempData["message"] as string;

                ViewBag.Users = _userDataAccess.GetAllUsers();
                ViewBag.Projects = _projectDataAccess.GetProjects();
                ViewBag.Tasks = _taskDataAccess.GetTasks();
                ViewBag.Shifts = _shiftDataAccess.GetShifts();
                ViewBag.EffortsToApprove = _effortDataAccess.GetPendingEffortsOfUsers();
                ViewBag.PendingLeaves = _leaveDataAccess.GetPendingLeaves();
                ViewBag.PendingShiftChanges = _shiftChangeDataAccess.GetPendingShiftChange();
                ViewBag.AssignedTasks = _assignTaskDataAccess.GetAllAssignedTasks();

                return View();
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while processing your request.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles user-related actions such as creating or updating a user.
        /// </summary>
        /// <param name="user">User model.</param>
        /// <param name="action">Action to perform.</param>
        /// <returns>Redirects to admin dashboard.</returns>
        [HttpPost]
        public ActionResult UserAction(User user, string action)
        {
            try
            {
                if (action == "create" || action == "update")
                {
                    if (!ModelState.IsValid)
                    {
                        return View("Index");
                    }

                    string message = (action == "create")
                        ? _userDataAccess.AddUser(user)
                        : _userDataAccess.UpdateUser(user);

                    TempData["message"] = message;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while performing user action.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles task-related actions such as assigning or updating a task.
        /// </summary>
        /// <param name="assignTask">AssignTask model.</param>
        /// <param name="action">Action to perform.</param>
        /// <returns>Redirects to admin dashboard.</returns>
        [HttpPost]
        public ActionResult TaskAction(AssignTask assignTask, string action)
        {
            try
            {
                if (action == "assign" || action == "update")
                {
                    if (!ModelState.IsValid)
                    {
                        return View("Index");
                    }

                    string message = (action == "assign")
                        ? _assignTaskDataAccess.AssignTask(assignTask)
                        : _assignTaskDataAccess.UpdateAssignTask(assignTask);

                    TempData["message"] = message;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while performing task action.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Approves an effort submission.
        /// </summary>
        /// <param name="effortid">ID of the effort to approve.</param>
        /// <returns>Redirects to admin dashboard.</returns>
        [HttpPost]
        public ActionResult ApproveEffort(int effortid)
        {
            try
            {
                string userName = _effortDataAccess.GetEffortUserName(effortid);
                string message = _effortDataAccess.ApproveEffort(effortid);

                if (message.Contains("Approved"))
                {
                    string subject = "Submitted Effort Status";
                    string body = $"Effort Approved for {userName}";
                    SendEmail(subject, body);
                }

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while approving effort.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Approves or rejects a leave request.
        /// </summary>
        /// <param name="leaveid">ID of the leave request.</param>
        /// <param name="action">Action to perform (approve or reject).</param>
        /// <returns>Redirects to admin dashboard.</returns>
        [HttpPost]
        public ActionResult ApproveLeave(int leaveid, string action)
        {
            try
            {
                string userName = _leaveDataAccess.GetLeaveUserName(leaveid);
                string message = _leaveDataAccess.ApproveOrRejectLeave(leaveid, action);

                string subject = "Submitted Leave Status";
                string body = (message.Contains("Approved"))
                    ? $"Leave Approved for {userName}"
                    : $"Leave Rejected for {userName}";

                SendEmail(subject, body);
                TempData["message"] = message;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while approving or rejecting leave.");
                return RedirectToAction("Error", "Home");
            }
        }
        /// <summary>
        /// Approves or rejects a shift change request.
        /// </summary>
        /// <param name="shiftChangeId">ID of the shift change request.</param>
        /// <param name="action">Action to perform (approve or reject).</param>
        /// <returns>Redirects to admin dashboard.</returns>
        [HttpPost]
        public ActionResult ShiftChange(int shiftChangeId, string action)
        {
            try
            {
                string userName = _shiftChangeDataAccess.GetShiftChangeUserName(shiftChangeId);
                string message = _shiftChangeDataAccess.ApproveOrRejectShiftChange(shiftChangeId, action);

                string subject = "Submitted Shift Change Status";
                string body = (message.Contains("Approved"))
                    ? $"Shift Change Approved for {userName}"
                    : $"Shift Change Rejected for {userName}";

                SendEmail(subject, body);
                TempData["message"] = message;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while processing shift change request.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles errors by logging them and storing an error message in TempData.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="errorMessage">A custom error message.</param>
        private void HandleError(Exception ex, string errorMessage)
        {
            _log.Error($"{errorMessage} {ex.Message}");
            TempData["ErrorMessage"] = errorMessage + ex;
        }
    }
}
