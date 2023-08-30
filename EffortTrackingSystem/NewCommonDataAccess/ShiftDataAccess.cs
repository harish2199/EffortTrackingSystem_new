using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewCommonDataAccess
{
    /// <summary>
    /// DataAccess class for managing Shift data.
    /// </summary>
    public class ShiftDataAccess : IShiftDataAccess
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the ShiftDataAccess class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public ShiftDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get a list of shifts.
        /// </summary>
        /// <returns>List of shifts.</returns>
        /// <exception cref="Exception">An error occurred while fetching shifts.</exception>
        public List<Common.Models.Shift> GetShifts()
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
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
