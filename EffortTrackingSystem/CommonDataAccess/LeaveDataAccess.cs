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
    public class LeaveDataAccess : ILeaveDataAccess
    {
        private readonly string _connectionString;
        public LeaveDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string SubmitLeave(Leave leave)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SubmitLeave", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@user_id", leave.UserId);
                        command.Parameters.AddWithValue("@date", leave.Date);
                        command.Parameters.AddWithValue("@reason", leave.Reason);
                        command.Parameters.AddWithValue("@status", leave.Status);

                        SqlParameter outputMessageParam = new SqlParameter("@output_message", SqlDbType.VarChar, 100);
                        outputMessageParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputMessageParam);

                        command.ExecuteNonQuery();

                        string outputMessage = outputMessageParam.Value.ToString();
                        return outputMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while submitting leave: " + ex.Message);
            }
        }

        public List<Leave> GetPendingLeaves()
        {
            List<Leave> pendingLeaves = new List<Leave>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetPendingLeaves", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Leave leave = new Leave
                                {
                                    LeaveId = Convert.ToInt32(reader["leave_id"]),
                                    UserId = Convert.ToInt32(reader["user_id"]),
                                    Date = Convert.ToDateTime(reader["date"]),
                                    Reason = reader["reason"].ToString(),
                                    Status = reader["status"].ToString(),
                                    User = new User
                                    {
                                        UserName = reader["user_name"].ToString()
                                    }
                                };

                                pendingLeaves.Add(leave);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching pending leaves: " + ex.Message);
            }

            return pendingLeaves;
        }

        public string ApproveOrRejectLeave(int leaveId, string newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ApproveOrRejectLeave", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@leave_id", leaveId);
                        command.Parameters.AddWithValue("@new_status", newStatus);

                        var outputMessage = command.Parameters.Add("@output_message", SqlDbType.NVarChar, 100);
                        outputMessage.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        return (string)outputMessage.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving or rejecting leave: " + ex.Message);
            }
        }
    }
}
