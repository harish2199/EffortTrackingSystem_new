﻿using CommonDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataAccess
{
    public interface IAssignTaskDataAccess
    {
        string AssignTask(AssignTask assignTask);
        List<AssignTask> GetAssignedTasks(int userId);
    }
}
