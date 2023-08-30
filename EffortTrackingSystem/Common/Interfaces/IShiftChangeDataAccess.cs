using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IShiftChangeDataAccess
    {
        string SubmitShiftChange(ShiftChange shiftChange);
        List<ShiftChange> GetPendingShiftChange();
        string ApproveOrRejectShiftChange(int shiftChangeId, string newStatus);
    }
}
