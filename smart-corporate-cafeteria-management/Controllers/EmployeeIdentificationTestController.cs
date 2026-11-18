using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Services;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class EmployeeIdentificationTestController : Controller
    {
        public IActionResult Index()
        {
            var service = new EmployeeIdentificationService();

            bool result1 = service.CanIdentifyEmployee(true, true);
            bool result2 = service.CanIdentifyEmployee(true, false);
            bool result3 = service.CanIdentifyEmployee(false, true);
            bool result4 = service.CanIdentifyEmployee(false, false);

            string output = $@"
                Employee Identification Test Results

                Test 1: ID + QR = {result1}
                Test 2: ID Only = {result2}
                Test 3: QR Only = {result3}
                Test 4: No ID, No QR = {result4}
            ";

            return Content(output);
        }
    }
}