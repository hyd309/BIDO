using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCoreWeb.Repository
{
    using System.Dynamic;

    public class UserRepository:IUserRepository
    {
        public dynamic GetUser(int id)
        {
            dynamic user = new ExpandoObject();
            user.Name = "胡亚东001";
            user.Id = id;
            return user;
        }
    }
}
