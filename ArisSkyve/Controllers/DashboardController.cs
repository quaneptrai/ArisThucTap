using ArisSkyve.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ArisSkyve.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
