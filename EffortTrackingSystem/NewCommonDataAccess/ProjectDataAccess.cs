using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Common;
using NewCommonDataAccess;

namespace NewCommonDataAccess
{
    public class ProjectDataAccess : IProjectDataAccess
    {
        private readonly string _connectionString;
        public ProjectDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Common.Models.Project> GetProjects()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
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
