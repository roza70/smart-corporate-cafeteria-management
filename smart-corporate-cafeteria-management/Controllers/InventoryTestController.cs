using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Services;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class InventoryTestController : Controller
    {
        public IActionResult Index()
        {
            var service = new InventoryService();

            string output = service.CheckStock();

            return Content(output);
        }
    }
}