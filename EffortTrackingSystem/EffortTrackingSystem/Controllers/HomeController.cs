using CommonDataAccess;
using Common.Models;
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
                string saltvalue = "";
                string ExistingHashedPassword = "";
                bool result = false;    

                User user = _userDataAccess.GetUserDetails(email);

                if (user != null)
                {
                    saltvalue = user.SaltValue;
                    ExistingHashedPassword = user.HashedPassword;
                    result = CompareHashedPasswords(password, ExistingHashedPassword, saltvalue);
                    if (result)
                    {
                        Session["Id"] = user.UserId;
                        Session["Name"] = user.UserName;
                        Session["Role"] = user.Role;

                        TempData["LoggedIn"] = "Welcome, " + user.UserName + "! You have successfully logged in.";
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                Admin admin = _adminDataAccess.GetAdminDetails(email);

                if(admin != null)
                {
                    saltvalue = admin.SaltValue;
                    ExistingHashedPassword = admin.HashedPassword;
                    result = CompareHashedPasswords(password, ExistingHashedPassword, saltvalue);
                    if (result)
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
                TempData["ErrorMessage"] = "An error occurred while processing your request." + ex;
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


        public byte[] GetHash(string PlainPassword, string Salt)
        {
            byte[] byteArray = Encoding.Unicode.GetBytes(String.Concat(Salt, PlainPassword));
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hashedBytes = sha256.ComputeHash(byteArray);
                return hashedBytes;
            }
        }
        public bool CompareHashedPasswords(string UserInputPassword, string ExistingHashedPassword, string Salt)
        {
            string UserInputtedHashedPassword = Convert.ToBase64String(GetHash(UserInputPassword, Salt));
            return ExistingHashedPassword == UserInputtedHashedPassword;
        }
    }
}
