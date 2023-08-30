using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCommonDataAccess
{
    public class AdminDataAccess : IAdminDataAccess
    {
        private readonly string _connectionString;

        public AdminDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Common.Models.Admin GetAdminDetails(string email)
        {
            try
            {
                using (EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities())
                {
                    var admin = (from u in _dbcontext.Admins.Where(a => a.admin_email == email)
                                 select new Common.Models.Admin
                                 {
                                     AdminId = u.admin_id,
                                     AdminName = u.admin_name,
                                     AdminEmail = u.admin_email,
                                     HashedPassword = u.hashed_password,
                                     Role = u.role,
                                     SaltValue = u.salt_value
                                 }).FirstOrDefault();

                    return admin;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching admin.", ex);
            }
        }

        public List<Common.Models.Admin> GetAllAdmins()
        {
            try
            {
                //EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities(_connectionString);
                EffortTrackingSystemEntities _dbcontext = new EffortTrackingSystemEntities();

                var admins = (from u in _dbcontext.Admins
                              select new Common.Models.Admin
                              {
                                  AdminId = u.admin_id,
                                  AdminName = u.admin_name,
                                  AdminEmail = u.admin_email,
                                  HashedPassword = u.hashed_password,
                                  Role = u.role,
                                  SaltValue = u.salt_value
                              }).ToList();

                return admins;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching admins.", ex);
            }
        }
    }
}
