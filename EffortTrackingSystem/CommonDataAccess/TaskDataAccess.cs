using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using CommonDataAccess.Models;

namespace CommonDataAccess
{
    public class TaskDataAccess : ITaskDataAccess
    {
        private readonly string _connectionString;
        public TaskDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetTasks", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Task task = new Task
                                {
                                    TaskId = Convert.ToInt32(reader["task_id"]),
                                    TaskName = reader["task_name"].ToString()
                                };

                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching tasks: " + ex.Message);
            }

            return tasks;
        }
    }
}
