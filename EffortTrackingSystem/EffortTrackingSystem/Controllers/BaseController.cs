using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    public class BaseController : Controller
    {
        protected readonly string _connectionString;
        //protected readonly ILog _log;

        public BaseController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings?["MyConnectionString"]?.ConnectionString;
            //_log = LogManager.GetLogger(typeof(BaseController));
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_connectionString == null)
            {
                filterContext.Result = RedirectToAction("ConnectionStringNotFound", "Error");
            }

            base.OnActionExecuting(filterContext);
        }

    }
}