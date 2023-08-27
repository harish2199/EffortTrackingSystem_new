using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using CommonDataAccess;
using log4net;

namespace EffortTrackingSystem.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string _connectionString;
        protected readonly AssignTaskDataAccess _assignTaskDataAccess;
        protected readonly EffortDataAccess _effortDataAccess;
        protected readonly LeaveDataAccess _leaveDataAccess;
        protected readonly ShiftChangeDataAccess _shiftChangeDataAccess;
        protected readonly ShiftDataAccess _shiftDataAccess;
        protected readonly ProjectDataAccess _projectDataAccess;
        protected readonly TaskDataAccess _taskDataAccess;
        protected readonly AdminDataAccess _adminDataAccess;
        protected readonly UserDataAccess _userDataAccess;
        protected readonly ILog _log;

        public BaseController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings?["MyConnectionString"]?.ConnectionString;
            _assignTaskDataAccess = new AssignTaskDataAccess(_connectionString);
            _effortDataAccess = new EffortDataAccess(_connectionString);
            _leaveDataAccess = new LeaveDataAccess(_connectionString);
            _shiftChangeDataAccess = new ShiftChangeDataAccess(_connectionString);
            _shiftDataAccess = new ShiftDataAccess(_connectionString);
            _projectDataAccess = new ProjectDataAccess(_connectionString);
            _taskDataAccess = new TaskDataAccess(_connectionString);
            _adminDataAccess = new AdminDataAccess(_connectionString);
            _userDataAccess = new UserDataAccess(_connectionString);
            _log = LogManager.GetLogger(typeof(BaseController));
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_connectionString == null)
            {
                filterContext.Result = RedirectToAction("ConnectionStringNotFound", "Error");
            }

            base.OnActionExecuting(filterContext);
        }

        protected void SendEmailTo(string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("harishdasu18@gmail.com", "cucujclxwipqhelz");
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("harishdasu18@gmail.com");
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.To.Add("veera.dasu18@gmail.com");

                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}
