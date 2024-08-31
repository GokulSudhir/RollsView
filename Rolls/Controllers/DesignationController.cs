using Microsoft.AspNetCore.Mvc;

namespace Rolls.Controllers
{
    public class DesignationController : Controller
    {
        public IActionResult Index()
        {
            var message = String.Empty;
            if (TempData["CrudStatus"] != null)
            {
                message = TempData["CrudStatus"].ToString();
            }

            DesignationAddEditVM indexObj = new DesignationAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }
    }
}
