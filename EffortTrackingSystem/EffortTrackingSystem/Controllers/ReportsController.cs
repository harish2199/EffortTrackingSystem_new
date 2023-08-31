using Common.Models;
using EffortTrackingSystem.Attributes;
using EffortTrackingSystem.Filters;
using log4net;
using NewCommonDataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Controller for generating and exporting reports.
    /// </summary>
    [MyAuthenticationFilter]
    public class ReportsController : BaseController
    {
        /// <summary>
        /// Displays the report based on filtering parameters.
        /// </summary>
        /// <param name="year">Year filter.</param>
        /// <param name="month">Month filter.</param>
        /// <param name="day">Day filter.</param>
        /// <param name="user">User filter.</param>
        /// <param name="project">Project filter.</param>
        /// <param name="export">Export action flag.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <returns>Report view.</returns>
        public ActionResult Index(int? year, int? month, int? day, int? userId, int? project, string export, int page = 1)
        {
            try
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                ViewBag.Year = year;
                ViewBag.Month = month;
                ViewBag.Day = day;
                ViewBag.User = userId;
                ViewBag.Project = project;

                bool isAdmin = IsAdminUser();
                int currentUserId = GetCurrentUserId();

                ViewBag.ShowUserDropdown = isAdmin;
                ViewBag.Users = _userDataAccess.GetAllUsers().ToList();
                ViewBag.Projects = _projectDataAccess.GetProjects().ToList();

                List<Common.Models.Effort> filteredEfforts = new List<Common.Models.Effort>();
                if (isAdmin)
                {
                    filteredEfforts = (userId == null)
                                        ? _effortDataAccess.GetFilteredEffortsOfAllUsers(year, month, day, project).ToList()
                                        : _effortDataAccess.GetFilteredEffortsOfUser(userId.Value, year, month, day, project).ToList();
                }
                else
                {
                    filteredEfforts = _effortDataAccess.GetFilteredEffortsOfUser(currentUserId, year, month, day, project).ToList();
                }

                filteredEfforts = SetupPagination(page, filteredEfforts);

                if (export != null)
                {
                    return ExportToCsv(filteredEfforts);
                }

                return View(filteredEfforts);
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while generating the report.");
                return RedirectToAction("Error", "Home");
            }
        }

        

        /// <summary>
        /// Exports data to CSV format.
        /// </summary>
        /// <param name="data">The data to export.</param>
        /// <returns>CSV file download.</returns>
        private ActionResult ExportToCsv(List<Common.Models.Effort> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    var csvBuilder = new StringBuilder();
                    csvBuilder.AppendLine("User,Date,Project,Task,Shift,Hours");

                    foreach (var item in data)
                    {
                        // Formatting data
                        var formatUser = item.AssignTask.User.UserName;
                        var formatDate = $"\t{item.SubmittedDate.ToString("yyyy-MM-dd")}";
                        var formatStartTime = item.Shift.StartTime.ToString(@"hh\:mm");
                        var formatEndTime = item.Shift.EndTime.ToString(@"hh\:mm");

                        // Appending to CSV
                        csvBuilder.AppendLine($"{formatUser},{formatDate},{item.AssignTask.Project.ProjectName},{item.AssignTask.Task.TaskName},{formatStartTime}-{formatEndTime},{item.HoursWorked}");
                    }

                    // Encoding and returning CSV file
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
                HandleError(ex, "An error occurred while exporting the report to CSV.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Gets the current user's ID from the session.
        /// </summary>
        /// <returns>The ID of the current user.</returns>
        private int GetCurrentUserId()
        {
            return (int)Session["Id"];
        }

        /// <summary>
        /// Sets up pagination information for the report view.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="efforts">The list of efforts to paginate.</param>
        private List<Common.Models.Effort> SetupPagination(int page, List<Common.Models.Effort> efforts)
        {
            try
            {
                int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfRecordsPerPageInReportsPage"]);
                int total = efforts.Count;
                var paginatedEfforts = efforts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.PageNumber = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
                return paginatedEfforts;
            }
            catch (Exception ex)
            {
                HandleError(ex, "An error occurred while setting up pagination.");
                return new List<Common.Models.Effort>();
            }
        }

        /// <summary>
        /// Checks whether the current user is an administrator.
        /// </summary>
        /// <returns>True if the user is an administrator, otherwise false.</returns>
        private bool IsAdminUser()
        {
            return Session["Role"].ToString().ToLower() == "admin";
        }

        /// <summary>
        /// Handles an error by logging it and storing an error message in TempData.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="errorMessage">A custom error message.</param>
        private void HandleError(Exception ex, string errorMessage)
        {
            _log.Error($"{errorMessage} {ex.Message}");
            TempData["ErrorMessage"] = errorMessage;
        }
    }
}
