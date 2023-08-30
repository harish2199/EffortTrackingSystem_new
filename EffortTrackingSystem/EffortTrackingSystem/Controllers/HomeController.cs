using Common.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace EffortTrackingSystem.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and related actions.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Displays the login view.
        /// </summary>
        /// <returns>Login view.</returns>
        public ActionResult Login()
        {
            try
            {
                ViewBag.LoginFailed = TempData["LoginFailed"] as string;
                ViewBag.LogOut = TempData["LogOut"] as string;
                return View();
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while loading the login view: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Processes user login.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                string saltValue = "";
                string existingHashedPassword = "";
                bool result = false;

                User user = _userDataAccess.GetUserDetails(email);

                if (user != null)
                {
                    saltValue = user.SaltValue;
                    existingHashedPassword = user.HashedPassword;
                    result = CompareHashedPasswords(password, existingHashedPassword, saltValue);
                    if (result)
                    {
                        Session["Id"] = user.UserId;
                        Session["Name"] = user.UserName;
                        Session["Role"] = user.Role;

                        TempData["LoggedIn"] = $"Welcome, {user.UserName}! You have successfully logged in.";
                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                TempData["LoginFailed"] = "Login Failed. Check credentials.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred while processing login: " + ex.Message);
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns>Action result.</returns>
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
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Displays an error view.
        /// </summary>
        /// <returns>Error view.</returns>
        public ActionResult Error()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            return View();
        }

        /// <summary>
        /// Displays a view when the connection string is not found.
        /// </summary>
        /// <returns>Connection string not found view.</returns>
        public ActionResult ConnectionStringNotFound()
        {
            ViewBag.ErrorMessage = "Something went wrong!";
            return View();
        }
        /// <summary>
        /// Computes the hash of a plain password combined with a salt.
        /// </summary>
        /// <param name="plainPassword">The plain password.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>The computed hash.</returns>
        private byte[] GetHash(string plainPassword, string salt)
        {
            byte[] byteArray = Encoding.Unicode.GetBytes(String.Concat(salt, plainPassword));
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hashedBytes = sha256.ComputeHash(byteArray);
                return hashedBytes;
            }
        }

        /// <summary>
        /// Compares a user's input password with an existing hashed password.
        /// </summary>
        /// <param name="userInputPassword">The user's input password.</param>
        /// <param name="existingHashedPassword">The existing hashed password.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>True if passwords match, otherwise false.</returns>
        private bool CompareHashedPasswords(string userInputPassword, string existingHashedPassword, string salt)
        {
            string userInputtedHashedPassword = Convert.ToBase64String(GetHash(userInputPassword, salt));
            return existingHashedPassword == userInputtedHashedPassword;
        }
    }
}
