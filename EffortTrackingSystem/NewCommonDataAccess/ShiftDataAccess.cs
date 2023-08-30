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
    public class ShiftDataAccess : IShiftDataAccess
    {
        private readonly string _connectionString;
        public ShiftDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Common.Models.Shift> GetShifts()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                //using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString))
                {
                    var shifts = (from s in _dbcontext.Shifts
                                  select new Common.Models.Shift
                                  {
                                      ShiftId = s.shift_id,
                                      ShiftName = s.shift_name,
                                      StartTime = s.start_time,
                                      EndTime = s.end_time
                                  }).ToList();

                    return shifts;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching shifts.", ex);
            }
        }
    }
}
