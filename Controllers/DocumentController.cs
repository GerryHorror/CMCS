// This is the boiler plate code for the Document Controller. This controller will be used to handle the documents. Functionality will be added to this controller in the future.

using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class DocumentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}