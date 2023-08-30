using System;
using Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ITaskDataAccess
    {
        List<Common.Models.Task> GetTasks();
    }
}
