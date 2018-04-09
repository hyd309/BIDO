using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableStoreWebApp.Repository
{
    public interface IUserRepository
    {
        dynamic GetUser(int id);
    }
}
