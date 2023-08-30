using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IAssignTaskDataAccess
    {
        string AssignTask(AssignTask assignTask);
        List<AssignTask> GetAllAssignedTasks();
        AssignTask GetPresentTaskForUser(int userId);
        string UpdateAssignTask(AssignTask assignTask);
    }
}
