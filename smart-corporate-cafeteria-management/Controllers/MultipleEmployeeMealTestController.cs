using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Services;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class MultipleEmployeeMealTestController : Controller
    {
        public IActionResult Index()
        {
            var service = new MultipleEmployeeMealService();

            string output = service.CheckEmployees();

            return Content(output);
        }
    }
}