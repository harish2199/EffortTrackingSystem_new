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
    public class AssignTaskDataAccess : IAssignTaskDataAccess
    {
        private readonly string _connectionString;
        public AssignTaskDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<AssignTask> GetAssignedTasksById(int userId)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var getAssignedTaskById = (from t in _dbcontext.Assign_Task.Where(a => a.user_id == userId)
                                               select new Common.Models.AssignTask
                                               {
                                                   AssignTaskId = t.assign_task_id,
                                                   User = new Common.Models.User
                                                   {
                                                       UserId = (int)t.user_id,
                                                       UserName = t.User.user_name
                                                   },
                                                   Project = new Common.Models.Project
                                                   {
                                                       ProjectId = (int)t.project_id,
                                                       ProjectName = t.Project.project_name
                                                   },
                                                   Task = new Common.Models.Task
                                                   {
                                                       TaskId = (int)t.task_id,
                                                       TaskName = t.Task.task_name,
                                                   },
                                                   Shift = new Common.Models.Shift
                                                   {
                                                       ShiftId = (int)t.shift_id,
                                                       ShiftName = t.Shift.shift_name
                                                   },
                                                   ShiftId = (int)t.shift_id,
                                                   StartDate = (DateTime)t.start_date,
                                                   EndDate = (DateTime)t.end_date,
                                                   Status = t.Status
                                               }).ToList();

                    return getAssignedTaskById;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching assigned tasks by user ID.", ex);
            }
        }

        public string AssignTask(AssignTask assignTask)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    //checks tasks dates
                    DateTime today = DateTime.Today;
                    if (assignTask.StartDate < today || assignTask.EndDate < today)
                    {
                        return "Start date and end date cannot be past dates.";
                    }

                    //checks if tasks already exsists
                    bool hasOverlappingTask = _dbcontext.Assign_Task.Any(task =>
                                                task.user_id == assignTask.UserId &&
                                                ((task.start_date <= assignTask.StartDate && task.end_date >= assignTask.StartDate) ||
                                                (task.start_date <= assignTask.EndDate && task.end_date >= assignTask.EndDate) ||
                                                (task.start_date >= assignTask.StartDate && task.end_date <= assignTask.EndDate) ||
                                                (task.start_date == assignTask.StartDate && task.end_date == assignTask.EndDate))); // Check for exact match

                    if (hasOverlappingTask)
                    {
                        return "An overlapping task already exists for the selected user and project.";
                    }

                    var newTask = new NewCommonDataAccess.Assign_Task
                    {
                        user_id = assignTask.UserId,
                        project_id = assignTask.ProjectId,
                        task_id = assignTask.TaskId,
                        shift_id = assignTask.ShiftId,
                        start_date = assignTask.StartDate,
                        end_date = assignTask.EndDate,
                        Status = assignTask.Status
                    };

                    _dbcontext.Assign_Task.Add(newTask);
                    _dbcontext.SaveChanges();

                    return "Task Added Successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while assigning the task.", ex);
            }
        }

        public string UpdateAssignTask(AssignTask assignTask)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var assignedTask = _dbcontext.Assign_Task.FirstOrDefault(a => a.assign_task_id == assignTask.AssignTaskId);
                    if (assignedTask == null)
                    {
                        return "Assigned Task mot found!";
                    }

                    assignedTask.assign_task_id = assignTask.AssignTaskId;
                    assignedTask.user_id = assignTask.UserId;
                    assignedTask.project_id = assignTask.ProjectId;
                    assignedTask.task_id = assignTask.TaskId;
                    assignedTask.shift_id = assignTask.ShiftId;
                    assignedTask.start_date = assignTask.StartDate;
                    assignedTask.end_date = assignTask.EndDate;

                    _dbcontext.SaveChanges();
                    return "Assigned Task Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating assigned task.", ex);
            }
        }

    }
}
