using Microsoft.AspNetCore.Mvc;

namespace Rolls.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            var message = String.Empty;
            if (TempData["CrudStatus"] != null)
            {
                message = TempData["CrudStatus"].ToString();
            }

            DepartmentsAddEditVM indexObj = new DepartmentsAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }
    }
}
