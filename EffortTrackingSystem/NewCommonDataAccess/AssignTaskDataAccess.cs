using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NewCommonDataAccess;
using Common;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing task assignments in an effort tracking system.
    /// </summary>
    public class AssignTaskDataAccess : IAssignTaskDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the AssignTaskDataAccess class.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        public AssignTaskDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves the current task assignment for a specified user based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose task assignment is to be retrieved.</param>
        /// <returns>An instance of AssignTask representing the current task assignment for the user.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching assigned tasks.</exception>
        public AssignTask GetPresentTaskForUser(int userId)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    DateTime today = DateTime.Now.Date;
                    var getAssignedTaskForUser = (from t in _dbcontext.Assign_Task.Where(a => a.user_id == userId && a.start_date <= today && a.end_date >= today)
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
                                               }).FirstOrDefault();

                    return getAssignedTaskForUser;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching assigned tasks by user ID.", ex);
            }
        }

        /// <summary>
        /// Retrieves all assigned tasks from the database.
        /// </summary>
        /// <returns>A list of AssignTask instances representing assigned tasks.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching assigned tasks.</exception>
        public List<AssignTask> GetAllAssignedTasks()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var getAssignedTaskById = (from t in _dbcontext.Assign_Task
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

        /// <summary>
        /// Assigns a task to a user.
        /// </summary>
        /// <param name="assignTask">The task assignment details to be added.</param>
        /// <returns>A message indicating the result of the assignment process.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while assigning the task.</exception>
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

        /// <summary>
        /// Updates an assigned task.
        /// </summary>
        /// <param name="assignTask">The updated task assignment details.</param>
        /// <returns>A message indicating the result of the update process.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while updating the assigned task.</exception>
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
