using CommonDataAccess;
using CommonDataAccess.Models;
using EffortTrackingSystem.Attributes;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    [UserAuthorize]
    public class DashboardController : BaseController
    {
        private readonly AssignTaskDataAccess _assignTaskDataAccess;
        private readonly EffortDataAccess _effortDataAccess;
        private readonly ILog _log;

        public DashboardController()
        {
            _assignTaskDataAccess = new AssignTaskDataAccess(_connectionString);
            _effortDataAccess = new EffortDataAccess(_connectionString);
            _log = LogManager.GetLogger(typeof(DashboardController));
        }

        public ActionResult Index()
        {
            try
            {
                ViewBag.LoggedIn = TempData["LoggedIn"] as string;
                int userId = (int)Session["Id"];
                DateTime dateTime = DateTime.Now;
                // DateTime dateTime = new DateTime(2023, 8, 13);

                List<AssignTask> presentTasks = _assignTaskDataAccess.GetAssignedTasks(userId)
                    .Where(a => a.StartDate <= dateTime && a.EndDate >= dateTime)
                    .ToList();

                List<Effort> previousEfforts = _effortDataAccess.GetEfforts()
                    .Where(e => e.AssignTask.User.UserId == userId && e.Status == "Approved")
                    .OrderByDescending(e => e.SubmittedDate)
                    .Take(7)
                    .ToList();

                ViewBag.PreviousEfforts = previousEfforts;
                SendEmailToAdmin(userId);
                return View(presentTasks);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred in the Dashboard: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return RedirectToAction("Error", "Home");
            }
        }

        private void SendEmailToAdmin(int? userId)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("harishdasu18@gmail.com", "cucujclxwipqhelz"),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("harishdasu18@gmail.com"),
                Subject = "New Effort Submission",
                Body = $"A new effort has been submitted by user {userId}."
            };

            mailMessage.To.Add("veera.dasu18@gmail.com");

            smtpClient.Send(mailMessage);
        }
    }
}
