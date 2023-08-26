using CommonDataAccess.Models;
using CommonDataAccess;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Linq;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    [AdminAuthorize]
    public class AdminController : BaseController
    {
        private readonly AssignTaskDataAccess _assignTaskDataAccess;
        private readonly EffortDataAccess _effortDataAccess;
        private readonly LeaveDataAccess _leaveDataAccess;
        private readonly ShiftChangeDataAccess _shiftChangeDataAccess;
        private readonly ShiftDataAccess _shiftDataAccess;
        private readonly ProjectDataAccess _projectDataAccess;
        private readonly TaskDataAccess _taskDataAccess;
        private readonly AdminDataAccess _adminDataAccess;
        private readonly UserDataAccess _userDataAccess;
        private readonly ILog _log;

        public AdminController()
        {
            _assignTaskDataAccess = new AssignTaskDataAccess(_connectionString);
            _effortDataAccess = new EffortDataAccess(_connectionString);
            _leaveDataAccess = new LeaveDataAccess(_connectionString);
            _shiftChangeDataAccess = new ShiftChangeDataAccess(_connectionString);
            _shiftDataAccess = new ShiftDataAccess(_connectionString);
            _projectDataAccess = new ProjectDataAccess(_connectionString);
            _taskDataAccess = new TaskDataAccess(_connectionString);
            _adminDataAccess = new AdminDataAccess(_connectionString);
            _userDataAccess = new UserDataAccess(_connectionString);
            _log = LogManager.GetLogger(typeof(AdminController));
        }

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
                    .OrderByDescending(e => e.SubmittedDate)
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
                string message = _effortDataAccess.ApproveEffort(effortid);

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
                string message = _leaveDataAccess.ApproveOrRejectLeave(leaveid, action);

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
                string message = _shiftChangeDataAccess.ApproveOrRejectShiftChange(shiftChangeId, action);

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
