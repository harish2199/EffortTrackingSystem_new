using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Filters
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                // Log the exception
                LogException(filterContext.Exception);

                // Redirect to error page or handle the error in a custom way
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "Error" }
                    });

                filterContext.ExceptionHandled = true;
            }
        }

        private void LogException(Exception ex)
        {
            ILog log = LogManager.GetLogger(typeof(GlobalExceptionFilter));

            log.Error("An error occurred: " + ex.Message, ex);
        }
    }
}