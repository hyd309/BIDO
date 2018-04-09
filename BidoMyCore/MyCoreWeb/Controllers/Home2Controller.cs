using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace MyCoreWeb.Controllers
{
    public class Home2Controller : Controller
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(Home2Controller));
        public IActionResult Index()
        {
            log.Info("开始执行.Home2Controller=>Index()..");
            return View();
        }
    }
}