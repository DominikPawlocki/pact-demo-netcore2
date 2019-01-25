using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;

namespace Pact.Consumer.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService carService;

        public HomeController(ICarService carService)
        {
            this.carService = carService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await carService.GetManufacturers();
            return View(result);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }
    }
}
