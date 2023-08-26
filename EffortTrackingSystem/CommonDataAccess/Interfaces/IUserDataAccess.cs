using CommonDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataAccess
{
    public interface IUserDataAccess
    {
        List<User> GetAllUsers();
        string AddUser(User user);
        string UpdateUser(User user);
    }
}
