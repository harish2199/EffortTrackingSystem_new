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
    public class ProjectDataAccess : IProjectDataAccess
    {
        private readonly string _connectionString;
        public ProjectDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetProjects", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Project project = new Project
                                {
                                    ProjectId = Convert.ToInt32(reader["project_id"]),
                                    ProjectName = reader["project_name"].ToString()
                                };

                                projects.Add(project);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching projects: " + ex.Message);
            }

            return projects;
        }
    }
}
