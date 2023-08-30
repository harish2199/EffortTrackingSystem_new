using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Attributes
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string userRole = ConfigurationManager.AppSettings["userRole"];
            return httpContext.Session["Id"] != null &&
                   httpContext.Session["Role"].ToString().ToLower() == userRole;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/Login");
        }
    }
}