using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableStoreWebApp.Services
{
    public interface IUserServices
    {
        string GetUserName(int id);
    }
}
