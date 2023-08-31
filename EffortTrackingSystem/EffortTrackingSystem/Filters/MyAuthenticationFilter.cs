using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace EffortTrackingSystem.Filters
{
    public class MyAuthenticationFilter : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if(filterContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }
    }
}