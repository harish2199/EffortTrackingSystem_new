using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataAccess
{
    public interface ITaskDataAccess
    {
        List<CommonDataAccess.Models.Task> GetTasks();
    }
}
