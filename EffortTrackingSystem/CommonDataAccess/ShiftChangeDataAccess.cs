using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommonDataAccess
{
    public class ShiftChangeDataAccess : IShiftChangeDataAccess
    {
        private readonly string _connectionString;
        public ShiftChangeDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string SubmitShiftChange(ShiftChange shiftChange)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SubmitShiftChange", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@user_id", shiftChange.UserId);
                        command.Parameters.AddWithValue("@assigned_shift_id", shiftChange.AssignedShiftId);
                        command.Parameters.AddWithValue("@date", shiftChange.Date);
                        command.Parameters.AddWithValue("@new_shift_id", shiftChange.NewShiftId);
                        command.Parameters.AddWithValue("@reason", shiftChange.Reason);
                        command.Parameters.AddWithValue("@status", shiftChange.Status);

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
                throw new Exception("An error occurred while submitting shift change: " + ex.Message);
            }
        }

        public List<ShiftChange> GetPendingShiftChange()
        {
            List<ShiftChange> pendingShiftChanges = new List<ShiftChange>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetPendingShiftChange", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ShiftChange shiftChange = new ShiftChange
                                {
                                    ShiftChangeId = Convert.ToInt32(reader["shift_change_id"]),
                                    UserId = Convert.ToInt32(reader["user_id"]),
                                    User = new User
                                    {
                                        UserName = reader["user_name"].ToString(),
                                    },
                                    AssignedShiftId = Convert.ToInt32(reader["assigned_shift_id"]),
                                    AssignedShift = new Shift
                                    {
                                        ShiftName = reader["assigned_shift_name"].ToString(),
                                    },
                                    Date = Convert.ToDateTime(reader["date"]),
                                    NewShiftId = Convert.ToInt32(reader["new_shift_id"]),
                                    NewShift = new Shift
                                    {
                                        ShiftName = reader["new_shift_name"].ToString(),
                                    },
                                    Reason = reader["reason"].ToString(),
                                    Status = reader["status"].ToString()
                                };

                                pendingShiftChanges.Add(shiftChange);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching pending shift changes: " + ex.Message);
            }

            return pendingShiftChanges;
        }

        public string ApproveOrRejectShiftChange(int shiftChangeId, string newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("ApproveOrRejectShiftChange", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@shiftChangeId", shiftChangeId);
                        command.Parameters.AddWithValue("@newStatus", newStatus);

                        var outputMessage = command.Parameters.Add("@outputMessage", SqlDbType.NVarChar, 100);
                        outputMessage.Direction = ParameterDirection.Output;

                        connection.Open();
                        command.ExecuteNonQuery();

                        return (string)outputMessage.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving or rejecting shift change: " + ex.Message);
            }
        }
    }
}
