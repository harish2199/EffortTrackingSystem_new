using CommonDataAccess.Models;
using CommonDataAccess;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace EffortTrackingSystem.Controllers
{
    [AdminAuthorize]
    public class AdminController : BaseController
    {
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
                ViewBag.EffortsToApprove = _effortDataAccess.GetEfforts()
                    .Where(e => e.Status == "Pending")
                    .ToList();
                ViewBag.PendingLeaves = _leaveDataAccess.GetPendingLeaves();
                ViewBag.PendingShiftChanges = _shiftChangeDataAccess.GetPendingShiftChange();

                return View();
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in Index method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult UserAction(User user, string action)
        {
            try
            {
                if (action == "create")
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("~/Views/Partials/_CreateUserModal.cshtml", user);
                    }

                    string message = _userDataAccess.AddUser(user);

                    TempData["message"] = message;
                    return RedirectToAction("Index");
                }
                else if (action == "update")
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("~/Views/Partials/_UpdateUserModal.cshtml", user);
                    }
                    string message = _userDataAccess.UpdateUser(user);

                    TempData["message"] = message;
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in UserAction method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while performing user action.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddTasks(AssignTask assignTask)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Partials/_AssignTaskModal.cshtml", assignTask);
                }

                string message = _assignTaskDataAccess.AssignTask(assignTask);

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in AddTasks method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while adding tasks.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveEffort(int effortid)
        {
            try
            {
                string userName = string.Empty;
                List<Effort> pendingefforts = _effortDataAccess.GetEfforts().Where(e => e.Status == "Pending").ToList();
                foreach (Effort effort in pendingefforts)
                {
                    if (effort.EffortId == effortid)
                    {
                        userName = effort.AssignTask.User.UserName;
                    }
                }

                string message = _effortDataAccess.ApproveEffort(effortid);
                if (message.Contains("Approved"))
                {
                    string subject = $"Submitted Effort Status";
                    String body = $"Effort Approved for {userName}";
                    SendEmailTo(subject, body);
                }

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in ApproveEffort method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while approving effort.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ApproveLeave(int leaveid, string action)
        {
            try
            {
                string userName = string.Empty;
                List<Leave> pendingleaves = _leaveDataAccess.GetPendingLeaves();
                foreach (Leave leave in pendingleaves)
                {
                    if(leave.LeaveId == leaveid)
                    {
                        userName = leave.User.UserName;
                    }
                }

                string message = _leaveDataAccess.ApproveOrRejectLeave(leaveid, action);
                string subject = $"Submitted Leave status";
                String body = null;
                if (message.Contains("Approved"))
                {
                     body = $"Leave Approved for {userName} ";
                }
                else if (message.Contains("Rejected"))
                {
                     body = $"Leave Rejected for {userName} ";
                }
                SendEmailTo(subject, body);


                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in ApproveLeave method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while approving or rejecting leave.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ShiftChange(int shiftChangeId, string action)
        {
            try
            {
                string userName = string.Empty;
                List<ShiftChange> pendingshiftchanges = _shiftChangeDataAccess.GetPendingShiftChange();
                foreach (ShiftChange shiftchange in pendingshiftchanges)
                {
                    if (shiftchange.ShiftChangeId == shiftChangeId)
                    {
                        userName = shiftchange.User.UserName;
                    }
                }

                string message = _shiftChangeDataAccess.ApproveOrRejectShiftChange(shiftChangeId, action);
                string subject = $"Submitted Shift Change status";
                String body = null;
                if (message.Contains("Approved"))
                {
                    body = $"Shift Change Approved for {userName} ";
                }
                else if (message.Contains("Rejected"))
                {
                    body = $"Shift Change Rejected for {userName} ";
                }
                SendEmailTo(subject, body);

                TempData["message"] = message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in ShiftChange method: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing shift change request.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
