using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Common;
//using CommonDataAccess;
using NewCommonDataAccess;
using log4net;
using System.Web.Routing;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Base controller for common functionality and properties.
    /// </summary>
    [ConnectionStringFilter]
    [ErrorHandlingFilter]
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
            _userDataAccess = new UserDataAccess(_connectionString);
            _log = LogManager.GetLogger(typeof(BaseController));
        }

        /// <summary>
        /// Sends an email with the specified subject and body.
        /// </summary>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body.</param>
        protected void SendEmail(string subject, string body)
        {
            try
            {
                string smtpServer = ConfigurationManager.AppSettings["smtpClient"];
                string mailSender = ConfigurationManager.AppSettings["MailSender"];
                string senderPassword = ConfigurationManager.AppSettings["SenderPassword"];
                string mailReceiver = ConfigurationManager.AppSettings["MailReceiver"];

                using (SmtpClient client = new SmtpClient(smtpServer))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(mailSender, senderPassword);
                    client.EnableSsl = true;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(mailSender);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.To.Add(mailReceiver);

                        client.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred:", ex);
                TempData["ErrorMessage"] = "An error occurred while sending the email!";
                RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Custom action filter to check the presence of a connection string.
        /// </summary>
        public class ConnectionStringFilterAttribute : ActionFilterAttribute
        {
            /// <summary>
            /// Executes before an action method is executed to check for the connection string.
            /// </summary>
            /// <param name="filterContext">The context for the action being executed.</param>
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string connectionString = ConfigurationManager.ConnectionStrings?["EffortTrackingSystemEntities"]?.ConnectionString;

                if (connectionString == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "ConnectionStringNotFound" }
                });
                }

                base.OnActionExecuting(filterContext);
            }
        }

        /// <summary>
        /// Custom exception filter for handling errors and exceptions.
        /// </summary>
        public class ErrorHandlingFilterAttribute : HandleErrorAttribute
        {
            /// <summary>
            /// Handles exceptions by logging errors and redirecting to an error page.
            /// </summary>
            /// <param name="filterContext">The context for the exception being handled.</param>
            public override void OnException(ExceptionContext filterContext)
            {
                var logger = LogManager.GetLogger(filterContext.Controller.GetType());

                logger.Error("An error occurred:", filterContext.Exception);

                filterContext.Controller.TempData["ErrorMessage"] = "An error occurred!";
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Home" },
                { "action", "Error" }
            });

                filterContext.ExceptionHandled = true;

                base.OnException(filterContext);
            }
        }
    }
}
