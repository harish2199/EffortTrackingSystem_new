using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Common.Models;
using Common;
using NewCommonDataAccess;

namespace NewCommonDataAccess
{
    public class TaskDataAccess : ITaskDataAccess
    {
        private readonly string _connectionString;
        public TaskDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Common.Models.Task> GetTasks()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var tasks = (from t in _dbcontext.Tasks
                                 select new Common.Models.Task
                                 {
                                     TaskId = t.task_id,
                                     TaskName = t.task_name
                                 }).ToList();

                    return tasks;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching tasks.", ex);
            }
        }
    }
}
