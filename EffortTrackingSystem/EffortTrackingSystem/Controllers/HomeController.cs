using CommonDataAccess;
using CommonDataAccess.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Login()
        {
            ViewBag.LoginFailed = TempData["LoginFailed"] as string;
            ViewBag.LogOut = TempData["LogOut"] as string;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                List<User> users = _userDataAccess.GetAllUsers();
                foreach (var user in users)
                {
                    if (user.UserEmail == email && user.HashedPassword == password)
                    {
                        Session["Id"] = user.UserId;
                        Session["Name"] = user.UserName;
                        Session["Role"] = user.Role;

                        TempData["LoggedIn"] = "Welcome, " + user.UserName + "! You have successfully logged in.";
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                List<Admin> admins = _adminDataAccess.GetAllAdmins();
                foreach (var admin in admins)
                {
                    if (admin.AdminEmail == email && admin.HashedPassword == password)
                    {
                        Session["Id"] = admin.AdminId;
                        Session["Name"] = admin.AdminName;
                        Session["Role"] = admin.Role;

                        TempData["LoggedIn"] = "Welcome, " + admin.AdminName + "! You have successfully logged in.";
                        return RedirectToAction("Index", "Admin");
                    }
                }

                TempData["LoginFailed"] = "Login Failed. Check credentials.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while processing login: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                TempData["LogOut"] = "You have been logged out.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while processing logout: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Error()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }
        public ActionResult ConnectionStringNotFound()
        {
            ViewBag.ErrorMessage = "Connection string not found in configuration.";
            return View();
        }
    }
}
