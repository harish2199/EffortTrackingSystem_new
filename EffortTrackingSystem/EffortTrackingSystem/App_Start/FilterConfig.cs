﻿using EffortTrackingSystem.Filters;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
