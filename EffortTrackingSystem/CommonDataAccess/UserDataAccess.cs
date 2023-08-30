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

namespace CommonDataAccess
{
    public class UserDataAccess : IUserDataAccess
    {
        private readonly string _connectionString;
        public UserDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetAllUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User
                                {
                                    UserId = Convert.ToInt32(reader["user_id"]),
                                    UserName = reader["user_name"].ToString(),
                                    Designation = reader["designation"].ToString(),
                                    UserEmail = reader["user_email"].ToString(),
                                    HashedPassword = reader["hashed_password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    SaltValue = reader["salt_value"].ToString()
                                };

                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching users: " + ex.Message);
            }

            return users;
        }

        public string AddUser(User user)
        {
            try
            {
                string SaltValue = GenerateSalt();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("AddUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        string hashedpasswordstring = Convert.ToBase64String(GetHash(user.HashedPassword, SaltValue));

                        command.Parameters.AddWithValue("@user_name", user.UserName);
                        command.Parameters.AddWithValue("@designation", user.Designation);
                        command.Parameters.AddWithValue("@user_email", user.UserEmail);
                        //command.Parameters.AddWithValue("@hashed_password", user.HashedPassword);
                        command.Parameters.AddWithValue("@hashed_password", hashedpasswordstring);
                        command.Parameters.AddWithValue("@role", user.Role);
                        command.Parameters.AddWithValue("@salt_value", SaltValue);


                        SqlParameter messageParameter = new SqlParameter("@message", SqlDbType.VarChar, 100);
                        messageParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(messageParameter);

                        connection.Open();
                        command.ExecuteNonQuery();

                        string message = command.Parameters["@message"].Value.ToString();
                        return message;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a user: " + ex.Message);
            }
        }

        public string UpdateUser(User user)
        {
            try
            {
                

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("UpdateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@user_id", user.UserId);
                        command.Parameters.AddWithValue("@user_name", user.UserName);
                        command.Parameters.AddWithValue("@designation", user.Designation);
                        command.Parameters.AddWithValue("@user_email", user.UserEmail);
                        command.Parameters.AddWithValue("@hashed_password", user.HashedPassword);
                        command.Parameters.AddWithValue("@role", user.Role);

                        var outputMessage = command.Parameters.Add("@output_message", SqlDbType.NVarChar, 100);
                        outputMessage.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        return (string)outputMessage.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating a user: " + ex.Message);
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

        public User GetUserDetails(string email)
        {
            throw new NotImplementedException();
        }
    }
}
