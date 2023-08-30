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
    public class UserDataAccess : IUserDataAccess
    {
        private readonly string _connectionString;
        public UserDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

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
        public List<Common.Models.User> GetAllUsers()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var users = (from a in _dbcontext.Users
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

        
        public string AddUser(Common.Models.User user)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
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

        public string UpdateUser(Common.Models.User user)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
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

        public string GenerateSalt()
        {
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return Convert.ToBase64String(salt);
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

        
    }
}
