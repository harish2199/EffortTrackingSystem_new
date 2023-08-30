using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface IUserDataAccess
    {
        User GetUserDetails(string email);
        List<User> GetAllUsers();
        string AddUser(User user);
        string UpdateUser(User user);
    }
}
