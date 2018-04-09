using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TableStoreWebApp.Models;

namespace TableStoreWebApp.Services
{
    using Microsoft.Extensions.Options;
    using TableStoreWebApp.Repository;
    using log4net;

    public class UserServices:IUserServices
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(UserServices));
        private readonly IUserRepository _userRepository;
        public TableStoreModel _tableStore;
        public UserServices(IUserRepository userRepository, IOptions<TableStoreModel> option)
        {
            _userRepository = userRepository;
            _tableStore = option.Value;
        }

        public string GetUserName(int id)
        {
            //log.Debug("UserServices=>GetUserName()");
            var user = _userRepository.GetUser(id);
            return user.Name;
        }
    }
}
