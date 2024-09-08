// This is the boiler plate code for the User Controller. This controller will be used to handle the users. Functionality will be added to this controller in the future.

using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}