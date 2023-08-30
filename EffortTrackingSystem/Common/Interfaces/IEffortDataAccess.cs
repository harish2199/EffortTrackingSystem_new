using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IEffortDataAccess
    {
        List<Effort> GetEfforts();
        string SubmitEffort(Effort effort, int userid);
        string ApproveEffort(int effortId);
        List<Effort> GetEffortsByDate(int? year = null, int? month = null, int? day = null);
    }
}
