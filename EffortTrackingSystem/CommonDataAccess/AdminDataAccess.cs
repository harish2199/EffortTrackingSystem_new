using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Common;

namespace CommonDataAccess
{
    public class AdminDataAccess : IAdminDataAccess
    {
        private readonly string _connectionString;
        public AdminDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Admin GetAdminDetails(string email)
        {
            throw new NotImplementedException();
        }

        public List<Admin> GetAllAdmins()
        {
            List<Admin> admins = new List<Admin>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetAllAdmins", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Admin admin = new Admin
                                {
                                    AdminId = Convert.ToInt32(reader["admin_id"]),
                                    AdminName = reader["admin_name"].ToString(),
                                    AdminEmail = reader["admin_email"].ToString(),
                                    HashedPassword = reader["hashed_password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    SaltValue = reader["salt_value"].ToString()
                                };

                                admins.Add(admin);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching admins. " + ex.Message);
            }

            return admins;
        }
    }
}