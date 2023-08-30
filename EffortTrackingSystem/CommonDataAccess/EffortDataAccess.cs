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
    public class EffortDataAccess : IEffortDataAccess
    {
        private readonly string _connectionString;
        public EffortDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Effort> GetEfforts()
        {
            List<Effort> approvedEfforts = new List<Effort>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetEfforts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Effort effort = new Effort
                                {
                                    EffortId = Convert.ToInt32(reader["effort_id"]),
                                    AssignTaskId = Convert.ToInt32(reader["assign_task_id"]),
                                    AssignTask = new AssignTask
                                    {
                                        User = new User
                                        {
                                            UserName = reader["user_name"].ToString(),
                                            UserId = Convert.ToInt32(reader["user_id"]),
                                        },
                                        Project = new Project
                                        {
                                            ProjectName = reader["project_name"].ToString(),
                                        },
                                        Task = new Task
                                        {
                                            TaskName = reader["task_name"].ToString(),
                                        }
                                    },
                                    Shift = new Shift
                                    {
                                        ShiftName = reader["shift_name"].ToString(),
                                    },

                                    HoursWorked = Convert.ToInt32(reader["hours_worked"]),
                                    SubmittedDate = Convert.ToDateTime(reader["submitted_date"]),
                                    Status = reader["status"].ToString(),
                                };

                                approvedEfforts.Add(effort);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching efforts: " + ex.Message);
            }

            return approvedEfforts;
        }

        public string SubmitEffort(Effort effort)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SubmitEffort", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@assign_task_id", effort.AssignTaskId);
                        command.Parameters.AddWithValue("@shift_id", effort.ShiftId);
                        command.Parameters.AddWithValue("@hours_worked", effort.HoursWorked);
                        command.Parameters.AddWithValue("@submitted_date", effort.SubmittedDate);
                        command.Parameters.AddWithValue("@status", effort.Status);

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
                throw new Exception("An error occurred while submitting effort: " + ex.Message);
            }
        }

        public string ApproveEffort(int effortId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ApproveEffort", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@effort_id", effortId);

                        var outputMessage = command.Parameters.Add("@output_message", SqlDbType.NVarChar, 100);
                        outputMessage.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        return (string)outputMessage.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving effort: " + ex.Message);
            }
        }

        public List<Effort> GetEffortsByDate(int? year = null, int? month = null, int? day = null)
        {
            List<Effort> approvedEfforts = new List<Effort>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetEffortsByDate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@year", year.HasValue ? (object)year.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@month", month.HasValue ? (object)month.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@day", day.HasValue ? (object)day.Value : DBNull.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Effort effort = new Effort
                                {
                                    EffortId = Convert.ToInt32(reader["effort_id"]),
                                    AssignTaskId = Convert.ToInt32(reader["assign_task_id"]),
                                    AssignTask = new AssignTask
                                    {
                                        UserId = Convert.ToInt32(reader["user_id"]),
                                        User = new User
                                        {
                                            UserName = reader["user_name"].ToString(),
                                        },
                                        Project = new Project
                                        {
                                            ProjectName = reader["project_name"].ToString(),
                                        },
                                        Task = new Task
                                        {
                                            TaskName = reader["task_name"].ToString(),
                                        }
                                    },
                                    Shift = new Shift
                                    {
                                        ShiftName = reader["shift_name"].ToString(),
                                        StartTime = TimeSpan.Parse(reader["start_time"].ToString()),
                                        EndTime = TimeSpan.Parse(reader["end_time"].ToString())
                                    },
                                    HoursWorked = Convert.ToInt32(reader["hours_worked"]),
                                    SubmittedDate = Convert.ToDateTime(reader["submitted_date"])
                                };

                                approvedEfforts.Add(effort);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching efforts by date: " + ex.Message);
            }

            return approvedEfforts;
        }

        public string SubmitEffort(Effort effort, int userid)
        {
            throw new NotImplementedException();
        }
    }
}
