using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Attributes
{
	public class AdminAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
            string adminRole = ConfigurationManager.AppSettings["adminRole"];
            return httpContext.Session["Id"] != null &&
				   httpContext.Session["Role"].ToString().ToLower() == adminRole;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectResult("~/Home/Login");
		}
	}
}