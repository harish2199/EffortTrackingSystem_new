using Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing effort tracking in an effort tracking system.
    /// </summary>
    public class EffortDataAccess : IEffortDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the EffortDataAccess class.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        public EffortDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves filtered efforts of a specific user based on provided criteria.
        /// </summary>
        /// <param name="userId">The ID of the user whose efforts are to be retrieved.</param>
        /// <param name="year">Optional year filter.</param>
        /// <param name="month">Optional month filter.</param>
        /// <param name="day">Optional day filter.</param>
        /// <param name="project">Optional project filter.</param>
        /// <returns>List of Effort instances representing filtered efforts.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching efforts.</exception>
        public List<Common.Models.Effort> GetFilteredEffortsOfUser(int userId, int? year = null, int? month = null, int? day = null, int? project = null)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var query = (from e in _dbcontext.Efforts.Where(e => e.Assign_Task.user_id == userId)
                                 where e.status == "Approved"
                                 && (!year.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Year == year.Value))
                                 && (!month.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Month == month.Value))
                                 && (!day.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Day == day.Value))
                                 && (!project.HasValue || (e.Assign_Task.project_id == project.Value))
                                 select new Common.Models.Effort
                                 {
                                     EffortId = e.effort_id,
                                     AssignTaskId = (int)e.assign_task_id,
                                     AssignTask = new AssignTask
                                     {
                                         User = new Common.Models.User
                                         {
                                             UserId = e.Assign_Task.User.user_id,
                                             UserName = e.Assign_Task.User.user_name,
                                         },
                                         Project = new Common.Models.Project
                                         {
                                             ProjectId = e.Assign_Task.Project.project_id,
                                             ProjectName = e.Assign_Task.Project.project_name,
                                         },
                                         Task = new Common.Models.Task
                                         {
                                             TaskName = e.Assign_Task.Task.task_name,
                                         }
                                     },
                                     Shift = new Common.Models.Shift
                                     {
                                         ShiftName = e.Shift.shift_name,
                                         StartTime = e.Shift.start_time,
                                         EndTime = e.Shift.end_time
                                     },

                                     HoursWorked = e.hours_worked,
                                     SubmittedDate = e.submitted_date.HasValue ? e.submitted_date.Value : DateTime.MinValue,
                                     Status = e.status
                                 }).ToList();

                    return query;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching efforts by date.", ex);
            }
        }
        
        /// <summary>
        /// Retrieves filtered efforts of all user based on provided criteria.
        /// </summary>
        /// <param name="year">Optional year filter.</param>
        /// <param name="month">Optional month filter.</param>
        /// <param name="day">Optional day filter.</param>
        /// <param name="project">Optional project filter.</param>
        /// <returns>List of Effort instances representing filtered efforts.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching efforts.</exception>
        public List<Common.Models.Effort> GetFilteredEffortsOfAllUsers(int? year = null, int? month = null, int? day = null, int? project = null)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {

                    var query = (from e in _dbcontext.Efforts.Where(e => e.Assign_Task.User.role.ToLower() == "user")
                                 where e.status == "Approved"
                                 && (!year.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Year == year.Value))
                                 && (!month.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Month == month.Value))
                                 && (!day.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Day == day.Value))
                                 && (!project.HasValue || (e.Assign_Task.project_id == project.Value))
                                 select new Common.Models.Effort
                                 {
                                     EffortId = e.effort_id,
                                     AssignTaskId = (int)e.assign_task_id,
                                     AssignTask = new AssignTask
                                     {
                                         User = new Common.Models.User
                                         {
                                             UserId = e.Assign_Task.User.user_id,
                                             UserName = e.Assign_Task.User.user_name,
                                         },
                                         Project = new Common.Models.Project
                                         {
                                             ProjectId = e.Assign_Task.Project.project_id,
                                             ProjectName = e.Assign_Task.Project.project_name,
                                         },
                                         Task = new Common.Models.Task
                                         {
                                             TaskName = e.Assign_Task.Task.task_name,
                                         }
                                     },
                                     Shift = new Common.Models.Shift
                                     {
                                         ShiftName = e.Shift.shift_name,
                                         StartTime = e.Shift.start_time,
                                         EndTime = e.Shift.end_time
                                     },

                                     HoursWorked = e.hours_worked,
                                     SubmittedDate = e.submitted_date.HasValue ? e.submitted_date.Value : DateTime.MinValue,
                                     Status = e.status
                                 }).ToList();

                    return query;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching filtered efforts.", ex);
            }
        }

        /// <summary>
        /// Retrieves the list of approved efforts associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose approved efforts are to be retrieved.</param>
        /// <returns>List of Effort instances representing approved efforts of the user.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching approved efforts.</exception>
        public List<Common.Models.Effort> GetApprovedEffortsOfUser(int userId)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var getEfforts = (from e in _dbcontext.Efforts.Where(e => e.Assign_Task.user_id == userId && e.status == "Approved")
                                      select new Common.Models.Effort
                                      {
                                          EffortId = e.effort_id,
                                          AssignTaskId = (int)e.assign_task_id,
                                          AssignTask = new AssignTask
                                          {
                                              User = new Common.Models.User
                                              {
                                                  UserId = e.Assign_Task.User.user_id,
                                                  UserName = e.Assign_Task.User.user_name,
                                              },
                                              Project = new Common.Models.Project
                                              {
                                                  ProjectName = e.Assign_Task.Project.project_name,
                                              },
                                              Task = new Common.Models.Task
                                              {
                                                  TaskName = e.Assign_Task.Task.task_name,
                                              }
                                          },
                                          Shift = new Common.Models.Shift
                                          {
                                              ShiftName = e.Shift.shift_name
                                          },

                                          HoursWorked = e.hours_worked,
                                          SubmittedDate = (DateTime)e.submitted_date,
                                          Status = e.status
                                      }).ToList();

                    return getEfforts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching efforts.", ex);
            }
        }

        /// <summary>
        /// Retrieves the list of pending efforts associated with all regular users.
        /// </summary>
        /// <returns>List of Effort instances representing pending efforts of regular users.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching pending efforts.</exception>
        public List<Common.Models.Effort> GetPendingEffortsOfUsers()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var getEfforts = (from e in _dbcontext.Efforts.Where(e => e.status == "Pending" && e.Assign_Task.User.role.ToLower() == "user")
                                      select new Common.Models.Effort
                                      {
                                          EffortId = e.effort_id,
                                          AssignTaskId = (int)e.assign_task_id,
                                          AssignTask = new AssignTask
                                          {
                                              User = new Common.Models.User
                                              {
                                                  UserId = e.Assign_Task.User.user_id,
                                                  UserName = e.Assign_Task.User.user_name,
                                              },
                                              Project = new Common.Models.Project
                                              {
                                                  ProjectName = e.Assign_Task.Project.project_name,
                                              },
                                              Task = new Common.Models.Task
                                              {
                                                  TaskName = e.Assign_Task.Task.task_name,
                                              }
                                          },
                                          Shift = new Common.Models.Shift
                                          {
                                              ShiftName = e.Shift.shift_name
                                          },

                                          HoursWorked = e.hours_worked,
                                          SubmittedDate = (DateTime)e.submitted_date,
                                          Status = e.status
                                      }).ToList();

                    return getEfforts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching efforts.", ex);
            }
        }

        /// <summary>
        /// Retrieves the user name associated with a specific effort ID.
        /// </summary>
        /// <param name="effortid">The ID of the effort for which the user name is to be retrieved.</param>
        /// <returns>The user name associated with the given effort ID.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while fetching the user name.</exception>
        public string GetEffortUserName(int effortid)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    string userName = (from u in _dbcontext.Efforts.Where(u => u.effort_id == effortid)
                                       select u.Assign_Task.User.user_name).FirstOrDefault();

                    return userName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching user Name who applied for leave.", ex);
            }
        }

        /// <summary>
        /// Submits an effort entry for a user.
        /// </summary>
        /// <param name="effort">The effort details to be submitted.</param>
        /// <param name="userid">The ID of the user submitting the effort.</param>
        /// <returns>A message indicating the result of the submission process.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while submitting the effort.</exception>
        public string SubmitEffort(Common.Models.Effort effort, int userid)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    // Check if an effort already exists for the same user and date
                    bool effortExists = _dbcontext.Efforts.Any(e =>
                        e.Assign_Task.User.user_id == userid &&
                        e.submitted_date == effort.SubmittedDate.Date
                    );
                    if (effortExists)
                    {
                        return "An effort has already been submitted for the date.";
                    }

                    // Check if a leave request 
                    var leaveRequest = _dbcontext.Leaves.FirstOrDefault(l =>
                        l.user_id == userid &&
                        l.date == effort.SubmittedDate.Date &&
                        (l.status == "Approved" || l.status == "Pending")
                    );
                    if (leaveRequest != null)
                    {
                        if (leaveRequest.status == "Approved")
                        {
                            return "Effort cannot be submitted on a day when a leave request is approved.";
                        }
                        else
                        {
                            return "Effort can not be submitted when leave request is pending.";
                        }
                    }

                    // Check if a shift change 
                    var shiftChangeRequest = _dbcontext.Shift_Change.FirstOrDefault(s =>
                        s.user_id == userid &&
                        s.date == effort.SubmittedDate.Date &&
                        (s.status == "Approved" || s.status == "Pending")
                    );
                    if (shiftChangeRequest != null)
                    {
                        if (shiftChangeRequest.status == "Approved")
                        {
                            if (effort.ShiftId != (int)shiftChangeRequest.new_shift_id)
                            {
                                return "Effort cannot be submitted with a shift different from the approved shift change.";
                            }
                        }
                        else
                        {
                            return "Effort cannot be submitted when shift change is pending.";
                        }
                    }
                    else
                    {
                        // Check if the entered shift ID matches the assigned shift ID
                        var assignedShiftId = _dbcontext.Assign_Task
                            .Where(a => a.assign_task_id == effort.AssignTaskId)
                            .Select(a => a.Shift.shift_id)
                            .FirstOrDefault();
                        if (effort.ShiftId != assignedShiftId)
                        {
                            return "Effort can only be submitted with the assigned shift ID.";
                        }
                    }

                    var newEffort = new NewCommonDataAccess.Effort
                    {
                        assign_task_id = effort.AssignTaskId,
                        shift_id = effort.ShiftId,
                        hours_worked = effort.HoursWorked,
                        submitted_date = effort.SubmittedDate,
                        status = effort.Status
                    };

                    _dbcontext.Efforts.Add(newEffort);
                    _dbcontext.SaveChanges();

                    return "Effort Submitted Successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while submitting the effort.", ex);
            }
        }

        /// <summary>
        /// Approves an effort entry.
        /// </summary>
        /// <param name="effortId">The ID of the effort entry to be approved.</param>
        /// <returns>A message indicating the result of the approval process.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while approving the effort.</exception>
        public string ApproveEffort(int effortId)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var existingeffort = _dbcontext.Efforts.FirstOrDefault(u => u.effort_id == effortId);
                    if (existingeffort == null)
                    {
                        return "Effort not found!";
                    }

                    // Update
                    existingeffort.status = "Approved";

                    _dbcontext.SaveChanges();

                    return "Effort Approved Successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving the effort.", ex);
            }
        }
    }
}
