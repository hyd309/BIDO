using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCoreWeb.Repository
{
    public interface IUserRepository
    {
        dynamic GetUser(int id);
    }
}
