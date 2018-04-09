using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using MyCoreWeb.Models;
using Microsoft.Extensions.Options;

namespace MyCoreWeb.Controllers
{
    public class HomeController : Controller
    {
        public ClassConifg Config;
        public MyCoreWeb.Services.IUserServices _IUserServices;

        public HomeController(IOptions<ClassConifg> option, MyCoreWeb.Services.IUserServices iUserServices)
        {
            Config = option.Value;
            _IUserServices = iUserServices;
        }

        public IActionResult Index()
        {
            string name=_IUserServices.GetUserName(12);
            return View(Config);
        }
    }
}