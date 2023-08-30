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
    public class AssignTaskDataAccess : IAssignTaskDataAccess
    {
        private readonly string _connectionString;
        public AssignTaskDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string AssignTask(AssignTask assignTask)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("AssignTask", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@user_id", assignTask.UserId);
                        command.Parameters.AddWithValue("@project_id", assignTask.ProjectId);
                        command.Parameters.AddWithValue("@task_id", assignTask.TaskId);
                        command.Parameters.AddWithValue("@shift_id", assignTask.ShiftId);
                        command.Parameters.AddWithValue("@start_date", assignTask.StartDate);
                        command.Parameters.AddWithValue("@end_date", assignTask.EndDate);
                        command.Parameters.AddWithValue("@status", assignTask.Status);

                        var outputMessage = command.Parameters.Add("@output_message", SqlDbType.NVarChar, 100);
                        outputMessage.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        return (string)outputMessage.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while assigning the task: " + ex.Message);
            }
        }

        public List<AssignTask> GetAssignedTasksById(int userId)
        {
            List<AssignTask> tasks = new List<AssignTask>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetAssignedTasksByUserId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AssignTask task = new AssignTask
                                {
                                    AssignTaskId = Convert.ToInt32(reader["assign_task_id"]),
                                    User = new User
                                    {
                                        UserId = Convert.ToInt32(reader["user_id"]),
                                    },
                                    Project = new Project
                                    {
                                        ProjectName = reader["project_name"].ToString()
                                    },
                                    Task = new Task
                                    {
                                        TaskName = reader["task_name"].ToString(),
                                    },
                                    Shift = new Shift
                                    {
                                        ShiftId = Convert.ToInt32(reader["shift_id"]),
                                        ShiftName = reader["shift_name"].ToString(),
                                    },
                                    ShiftId = Convert.ToInt32(reader["shift_id"]),
                                    StartDate = Convert.ToDateTime(reader["start_date"]),
                                    EndDate = Convert.ToDateTime(reader["end_date"]),
                                    Status = reader["Status"].ToString()
                                };

                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching assigned tasks: " + ex.Message);
            }

            return tasks;
        }

        public string UpdateAssignTask(AssignTask assignTask)
        {
            throw new NotImplementedException();
        }
    }
}
