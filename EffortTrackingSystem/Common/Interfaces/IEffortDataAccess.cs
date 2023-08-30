using Common.Models;
using System.Collections.Generic;

namespace Common
{
    public interface IEffortDataAccess
    {
        List<Common.Models.Effort> GetFilteredEffortsOfUser(int userId, int? year = null, int? month = null, int? day = null, int? project = null);
        List<Common.Models.Effort> GetFilteredEffortsOfAllUsers(int? year = null, int? month = null, int? day = null, int? project = null);
        List<Common.Models.Effort> GetApprovedEffortsOfUser(int userId);
        List<Common.Models.Effort> GetPendingEffortsOfUsers();
        string GetEffortUserName(int effortid);
        string SubmitEffort(Common.Models.Effort effort, int userid);
        string ApproveEffort(int effortId);
    }
}
