using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealTimeChat.Data;
using RealTimeChat.Hubs;
using RealTimeChat.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<User> _userRepo;

        public HomeController(ILogger<HomeController> logger, IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AcceptVerbs("Get", "Post")]
        public async ValueTask<IActionResult> CheckNickname(string nickname)
        {
            if (await _userRepo.FindFirstOrDefault(x => x.Nickname == nickname) != null)
                return Json(false);
            return Json(true);
        }
    }
}
