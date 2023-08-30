using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using NewCommonDataAccess;

namespace NewCommonDataAccess
{
    public class ShiftChangeDataAccess : IShiftChangeDataAccess
    {
        private readonly string _connectionString;
        public ShiftChangeDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ShiftChange> GetPendingShiftChange()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var pendingShiftChanges = (from s in _dbcontext.Shift_Change.Where(s => s.status == "Pending")
                                               select new Common.Models.ShiftChange
                                               {
                                                   ShiftChangeId = s.shift_Change_id,
                                                   UserId = (int)s.user_id,
                                                   User = new Common.Models.User
                                                   {
                                                       UserName = s.User.user_name
                                                   },
                                                   AssignedShiftId = s.shift_Change_id,
                                                   AssignedShift = new Common.Models.Shift
                                                   {
                                                       ShiftName = s.Shift.shift_name
                                                   },
                                                   Date = s.date,
                                                   NewShiftId = (int)s.new_shift_id,
                                                   NewShift = new Common.Models.Shift
                                                   {
                                                       ShiftName = s.Shift.shift_name
                                                   },
                                                   Reason = s.reason,
                                                   Status = s.status
                                               }).ToList();

                    return pendingShiftChanges;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching pending shift changes.", ex);
            }
        }

        public string SubmitShiftChange(ShiftChange shiftChange)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    bool isSameShift = _dbcontext.Assign_Task.Any(a =>
                                a.user_id == shiftChange.UserId && a.shift_id == shiftChange.NewShiftId);
                    if (isSameShift)
                    {
                        return "Cannot submit a shift change request to the same shift that is currently assigned.";
                    }
                    // One shift change per day
                    bool shiftChangeExists = _dbcontext.Shift_Change.Any(s =>
                        s.user_id == shiftChange.UserId && s.date == shiftChange.Date
                    );
                    if (shiftChangeExists)
                    {
                        return "A shift change request already exists for the selected date.";
                    }

                    // Check if effort
                    bool effortSubmitted = _dbcontext.Efforts.Any(e =>
                        e.Assign_Task.user_id == shiftChange.UserId && e.submitted_date == shiftChange.Date
                    );
                    if (effortSubmitted)
                    {
                        return "Effort has already been submitted for the selected date.";
                    }

                    // Check if a leave request 
                    var leaveRequest = _dbcontext.Leaves.FirstOrDefault(l =>
                        l.user_id == shiftChange.UserId  &&
                        l.date == shiftChange.Date &&
                        (l.status == "Approved" || l.status == "Pending")
                    );
                    if (leaveRequest != null)
                    {
                        if (leaveRequest.status == "Approved")
                        {
                            return "Shift change cannot be submitted when a leave request is approved.";
                        }
                        else
                        {
                            return "Shift change can not be submitted when leave request is pending.";
                        }
                    }
                    /*// Check leave request
                    bool leaveSubmitted = _dbcontext.Leaves.Any(l =>
                        l.user_id == shiftChange.UserId && l.date == shiftChange.Date
                    );
                    if (leaveSubmitted)
                    {
                        return "A leave request has been submitted for the selected date.";
                    }*/

                    var newShiftChange = new NewCommonDataAccess.Shift_Change
                    {
                        user_id = shiftChange.UserId,
                        assigned_shift_id = shiftChange.AssignedShiftId,
                        date = shiftChange.Date,
                        new_shift_id = shiftChange.NewShiftId,
                        reason = shiftChange.Reason,
                        status = shiftChange.Status
                    };

                    _dbcontext.Shift_Change.Add(newShiftChange);
                    _dbcontext.SaveChanges();

                    return "Shift Change submitted successfully";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while submitting shift change.", ex);
            }
        }

        public string ApproveOrRejectShiftChange(int shiftChangeId, string newStatus)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var existingShiftChange = _dbcontext.Shift_Change.FirstOrDefault(u => u.shift_Change_id == shiftChangeId);
                    if (existingShiftChange == null)
                    {
                        return "Shift Change not found!";
                    }

                    // Update
                    existingShiftChange.status = newStatus;

                    _dbcontext.SaveChanges();
                    if (newStatus == "Approved")
                    {
                        return "Shift Change Approved Successfully";
                    }
                    else
                    {
                        return "Shift Change Rejected";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving or rejecting shift change.", ex);
            }
        }

    }
}
