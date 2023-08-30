using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing Project data.
    /// </summary>
    public class ProjectDataAccess : IProjectDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the ProjectDataAccess class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public ProjectDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get a list of projects.
        /// </summary>
        /// <returns>List of projects.</returns>
        /// <exception cref="Exception">An error occurred while fetching projects.</exception>
        public List<Common.Models.Project> GetProjects()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var projects = (from p in _dbcontext.Projects
                                    select new Common.Models.Project
                                    {
                                        ProjectId = p.project_id,
                                        ProjectName = p.project_name,
                                    }).ToList();

                    return projects;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching projects.", ex);
            }
        }
    }
}
