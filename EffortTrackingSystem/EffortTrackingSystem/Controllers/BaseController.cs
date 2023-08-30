using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Common;
//using CommonDataAccess;
using NewCommonDataAccess;
using log4net;

namespace EffortTrackingSystem.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string _connectionString;
        protected readonly IAssignTaskDataAccess _assignTaskDataAccess;
        protected readonly IEffortDataAccess _effortDataAccess;
        protected readonly ILeaveDataAccess _leaveDataAccess;
        protected readonly IShiftChangeDataAccess _shiftChangeDataAccess;
        protected readonly IShiftDataAccess _shiftDataAccess;
        protected readonly IProjectDataAccess _projectDataAccess;
        protected readonly ITaskDataAccess _taskDataAccess;
        protected readonly IAdminDataAccess _adminDataAccess;
        protected readonly IUserDataAccess _userDataAccess;
        protected readonly ILog _log;

        public BaseController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings?["EffortTrackingSystemEntities"]?.ConnectionString;
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
                filterContext.Result = RedirectToAction("ConnectionStringNotFound", "Home");
            }

            base.OnActionExecuting(filterContext);
        }

        protected void SendEmailTo(string subject, string body)
        {
            string smtpclent = ConfigurationManager.AppSettings["smtpClient"];
            string mailSender = ConfigurationManager.AppSettings["MailSender"];
            string senderPassword = ConfigurationManager.AppSettings["SenderPassword"];
            string mailReceiver = ConfigurationManager.AppSettings["MailReceiver"];

            using (SmtpClient smtpClient = new SmtpClient(smtpclent))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(mailSender, senderPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(mailSender);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.To.Add(mailReceiver);

                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}
