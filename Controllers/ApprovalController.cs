/* This is the boiler plate code for the Approval Controller. This controller will be used to handle the approval of claims. Functionality will be added to this controller in the future. */

using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}