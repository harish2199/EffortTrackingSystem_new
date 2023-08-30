using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Common;
using NewCommonDataAccess;
using System.Data.Entity;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing user data.
    /// </summary>
    public class UserDataAccess : IUserDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the UserDataAccess class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public UserDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get user details based on email.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>User details.</returns>
        /// <exception cref="Exception">An error occurred while fetching user.</exception>
        public Common.Models.User GetUserDetails(string email)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var user = (from a in _dbcontext.Users.Where(u => u.user_email == email)
                                select new Common.Models.User
                                {
                                    UserId = a.user_id,
                                    UserName = a.user_name,
                                    Designation = a.designation,
                                    UserEmail = a.user_email,
                                    HashedPassword = a.hashed_password,
                                    Role = a.role,
                                    SaltValue = a.salt_value
                                }).FirstOrDefault();

                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching user.", ex);
            }
        }

        /// <summary>
        /// Get all users with the "user" role.
        /// </summary>
        /// <returns>List of users with "user" role.</returns>
        /// <exception cref="Exception">An error occurred while fetching users.</exception>
        public List<Common.Models.User> GetAllUsers()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var users = (from a in _dbcontext.Users.Where(u => u.role.ToLower() == "user")
                                 select new Common.Models.User
                                 {
                                     UserId = a.user_id,
                                     UserName = a.user_name,
                                     Designation = a.designation,
                                     UserEmail = a.user_email,
                                     HashedPassword = a.hashed_password,
                                     Role = a.role,
                                     SaltValue = a.salt_value
                                 }).ToList();

                    return users;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching users.", ex);
            }
        }

        /// <summary>
        /// Add a new user.
        /// </summary>
        /// <param name="user">User object to be added.</param>
        /// <returns>Message indicating the result of the operation.</returns>
        /// <exception cref="Exception">An error occurred while adding a user.</exception>
        public string AddUser(Common.Models.User user)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    string saltValue = GenerateSalt();
                    byte[] hashedPassword = GetHash(user.HashedPassword, saltValue);

                    bool userExists = _dbcontext.Users.Any(u => u.user_email == user.UserEmail);
                    if (userExists)
                    {
                        return "User with the same email already exists!";
                    }

                    var newUser = new NewCommonDataAccess.User
                    {
                        user_id = user.UserId,
                        user_name = user.UserName,
                        designation = user.Designation,
                        user_email = user.UserEmail,
                        hashed_password = Convert.ToBase64String(hashedPassword),
                        role = user.Role,
                        salt_value = saltValue
                    };

                    _dbcontext.Users.Add(newUser);
                    _dbcontext.SaveChanges();

                    return "User added successfully!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a user.", ex);
            }
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="user">User object to be updated.</param>
        /// <returns>Message indicating the result of the operation.</returns>
        /// <exception cref="Exception">An error occurred while updating a user.</exception>
        public string UpdateUser(Common.Models.User user)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var existingUser = _dbcontext.Users.FirstOrDefault(u => u.user_email == user.UserEmail);
                    if (existingUser == null)
                    {
                        return "User not found!";
                    }

                    existingUser.user_name = user.UserName;
                    existingUser.designation = user.Designation;
                    existingUser.user_email = user.UserEmail;
                    existingUser.role = user.Role;

                    _dbcontext.SaveChanges();

                    return "User updated successfully!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating a user.", ex);
            }
        }

        /// <summary>
        /// Generate a random salt value.
        /// </summary>
        /// <returns>Generated salt value.</returns>
        /// <exception cref="Exception">Error generating salt.</exception>
        public string GenerateSalt()
        {
            try
            {
                byte[] salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating salt.", ex);
            }
        }

        /// <summary>
        /// Compute the hash of a password combined with a salt value.
        /// </summary>
        /// <param name="plainPassword">Plain text password.</param>
        /// <param name="salt">Salt value.</param>
        /// <returns>Computed hash value.</returns>
        /// <exception cref="Exception">Error getting hash.</exception>
        public byte[] GetHash(string plainPassword, string salt)
        {
            try
            {
                byte[] byteArray = Encoding.Unicode.GetBytes(String.Concat(salt, plainPassword));
                using (SHA256Managed sha256 = new SHA256Managed())
                {
                    byte[] hashedBytes = sha256.ComputeHash(byteArray);
                    return hashedBytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting hash.", ex);
            }
        }
    }
}
