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
    public class EffortDataAccess : IEffortDataAccess
    {
        private readonly string _connectionString;
        public EffortDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Common.Models.Effort> GetEfforts()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var getEfforts = (from e in _dbcontext.Efforts
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
        public List<Common.Models.Effort> GetEffortsByDate(int? year = null, int? month = null, int? day = null)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var query = (from e in _dbcontext.Efforts
                                where e.status == "Approved"
                                && (!year.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Year == year.Value))
                                && (!month.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Month == month.Value))
                                && (!day.HasValue || (e.submitted_date.HasValue && e.submitted_date.Value.Day == day.Value))
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
