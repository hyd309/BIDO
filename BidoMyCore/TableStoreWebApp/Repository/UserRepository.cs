using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableStoreWebApp.Repository
{
    using System.Dynamic;
    using log4net;

    public class UserRepository:IUserRepository
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(UserRepository));
        public dynamic GetUser(int id)
        {
            dynamic user = new ExpandoObject();
            user.Name = "胡亚东001";
            user.Id = id;

            //log.Info(user.Name);
            return user;
        }
    }
}
