using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommonDataAccess
{
    public class ShiftDataAccess : IShiftDataAccess
    {
        private readonly string _connectionString;
        public ShiftDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Shift> GetShifts()
        {
            List<Shift> shifts = new List<Shift>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetShifts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Shift shift = new Shift
                                {
                                    ShiftId = (int)reader["shift_id"],
                                    ShiftName = (string)reader["shift_name"],
                                    StartTime = (TimeSpan)reader["start_time"],
                                    EndTime = (TimeSpan)reader["end_time"]
                                };

                                shifts.Add(shift);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching shifts: " + ex.Message);
            }

            return shifts;
        }
    }
}
