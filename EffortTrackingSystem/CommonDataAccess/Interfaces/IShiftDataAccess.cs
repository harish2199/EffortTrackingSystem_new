using CommonDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataAccess
{
    public interface IShiftDataAccess
    {
        List<Shift> GetShifts();
    }
}
