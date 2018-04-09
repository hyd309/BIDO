using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyCoreWeb.Models;

namespace MyCoreWeb.Services
{
    using Microsoft.Extensions.Options;
    using MyCoreWeb.Repository;

    public class UserServices:IUserServices
    {
        private readonly IUserRepository _userRepository;
        public ClassConifg _Config;
        public UserServices(IUserRepository userRepository, IOptions<ClassConifg> option)
        {
            _userRepository = userRepository;
            _Config = option.Value;
        }

        public string GetUserName(int id)
        {
            var user = _userRepository.GetUser(id);
            return user.Name;
        }
    }
}
