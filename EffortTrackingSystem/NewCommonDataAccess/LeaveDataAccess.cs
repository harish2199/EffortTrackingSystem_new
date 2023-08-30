using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing Leave data.
    /// </summary>
    public class LeaveDataAccess : ILeaveDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the LeaveDataAccess class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public LeaveDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get a list of pending leave requests.
        /// </summary>
        /// <returns>List of pending leave requests.</returns>
        /// <exception cref="Exception">An error occurred while fetching pending leaves.</exception>
        public List<Common.Models.Leave> GetPendingLeaves()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var pendingLeaves = (from l in _dbcontext.Leaves.Where(l => l.status == "Pending")
                                         select new Common.Models.Leave
                                         {
                                             LeaveId = l.leave_id,
                                             UserId = (int)l.user_id,
                                             Date = (DateTime)l.date,
                                             Reason = l.reason,
                                             Status = l.status,
                                             User = new Common.Models.User
                                             {
                                                 UserName = l.User.user_name
                                             }
                                         }).ToList();

                    return pendingLeaves;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching pending leaves.", ex);
            }
        }

        /// <summary>
        /// Submit a leave request.
        /// </summary>
        /// <param name="leave">The leave object to be submitted.</param>
        /// <returns>Submission status message.</returns>
        /// <exception cref="Exception">An error occurred while submitting leave.</exception>
        public string SubmitLeave(Common.Models.Leave leave)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    // One leave per day
                    bool leaveExists = _dbcontext.Leaves.Any(l =>
                        l.user_id == leave.UserId && l.date == leave.Date
                    );
                    if (leaveExists)
                    {
                        return "A leave request already exists for the selected date.";
                    }

                    // Check if effort 
                    bool effortSubmitted = _dbcontext.Efforts.Any(e =>
                        e.Assign_Task.user_id == leave.UserId && e.submitted_date == leave.Date
                    );
                    if (effortSubmitted)
                    {
                        return "Effort has already been submitted for the selected date.";
                    }

                    // Check if a shift change 
                    bool shiftChangeSubmitted = _dbcontext.Shift_Change.Any(s =>
                        s.user_id == leave.UserId && s.date == leave.Date
                    );
                    if (shiftChangeSubmitted)
                    {
                        return "A shift change has been submitted for the selected date.";
                    }

                    var newLeave = new NewCommonDataAccess.Leave
                    {
                        user_id = leave.UserId,
                        date = leave.Date,
                        reason = leave.Reason,
                        status = leave.Status
                    };

                    _dbcontext.Leaves.Add(newLeave);
                    _dbcontext.SaveChanges();

                    return "Leave submitted successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while submitting leave.", ex);
            }
        }

        /// <summary>
        /// Approve or reject a leave request.
        /// </summary>
        /// <param name="leaveId">The ID of the leave request.</param>
        /// <param name="newStatus">The new status for the leave request.</param>
        /// <returns>Status message of the operation.</returns>
        /// <exception cref="Exception">An error occurred while approving or rejecting leave.</exception>
        public string ApproveOrRejectLeave(int leaveId, string newStatus)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var existingLeave = _dbcontext.Leaves.FirstOrDefault(u => u.leave_id == leaveId);
                    if (existingLeave == null)
                    {
                        return "Leave not found!";
                    }

                    // Update
                    existingLeave.status = newStatus;

                    _dbcontext.SaveChanges();

                    if (newStatus == "Approved")
                    {
                        return "Leave Approved Successfully";
                    }
                    else
                    {
                        return "Leave Rejected";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving or rejecting leave.", ex);
            }
        }

        /// <summary>
        /// Get the user name associated with a leave request.
        /// </summary>
        /// <param name="leaveid">The ID of the leave request.</param>
        /// <returns>User name of the leave applicant.</returns>
        /// <exception cref="Exception">An error occurred while fetching user Name who applied for leave.</exception>
        public string GetLeaveUserName(int leaveid)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    string userName = (from u in _dbcontext.Leaves.Where(u => u.leave_id == leaveid)
                                       select u.User.user_name).FirstOrDefault();

                    return userName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching user Name who applied for leave.", ex);
            }
        }
    }
}
