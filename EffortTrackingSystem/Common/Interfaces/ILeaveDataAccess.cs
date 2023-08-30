using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ILeaveDataAccess
    {
        string SubmitLeave(Leave leave);
        List<Leave> GetPendingLeaves();
        string ApproveOrRejectLeave(int leaveId, string newStatus);
    }
}
