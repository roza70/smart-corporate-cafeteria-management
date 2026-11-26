
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Procurement Staff")]
    public class ProcurementStaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
