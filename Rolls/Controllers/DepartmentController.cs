using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Rolls.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DbContext _dbContext;

        private readonly IDepartmentCaller _caller;
        public DepartmentController(IDepartmentCaller caller)
        {
            _caller = caller;
        }

        public IActionResult Index()
        {
            var message = String.Empty;
            if (TempData["CrudStatus"] != null)
            {
                message = TempData["CrudStatus"].ToString();
            }

            DepartmentAddEditVM indexObj = new DepartmentAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }


        [Route("/Department/GetDepartmentsAsync")]
        public async Task<JsonResult> GetDepartmentsAsync()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault().ToUpper();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            IList<Department> dataObj = new List<Department>();
            try
            {
                var result = await _caller.GetDepartmentsAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new Department()
                        {
                            department_id = item.department_id,
                            department_name = item.department_name,
                            department_code = item.department_code
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DepartmentController/GetDepartmentssAsync Err:{err.GetBaseException().Message}");
            }

            IEnumerable<Department> departmentObj = dataObj;
            if (!string.IsNullOrEmpty(searchValue))
            {
                departmentObj = departmentObj.Where(m => m.department_name.Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                departmentObj = departmentObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
            }

            recordsTotal = departmentObj.Count();
            var data = departmentObj.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        [HttpPost("/Department/AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentAddEditVM dataObj)
        {
            var response = 0;
            long departmentId = 0;
            string departmentName = dataObj.department_name;
            string departmentCode = dataObj.department_code;
            if (dataObj.department_id != null)
            {
                departmentId = dataObj.department_id.Value;
                departmentName = dataObj.department_name_edit;
                departmentCode = dataObj.department_code_edit;

            }
            IsExistsVM isExistsObj = new IsExistsVM() { str1 = departmentName, str2 = departmentCode, id = departmentId.ToString() };
            var result1 = await _caller.DepartmentNameExistsAsync(isExistsObj);
            
            var jsonObj1 = JObject.Parse(result1);

            if ((int)jsonObj1["status"] == 200)
            {
                var dataObj1 = (dynamic)jsonObj1["dataObj"];
                if (dataObj1 == null)
                {
                    response = 1;
                }
                else
                {
                    if (dataObj1.record_status == "DELETED")
                    {
                        response = 2;
                    }
                    else
                    {
                        response = 3;
                    }
                }
            }

            List<string> errors = new List<string>();
            if (response == 0)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        //var result = "";
                        //if (bankId == 0)
                        //{
                        dataObj.record_status = "ACTIVE";
                        var result = await _caller.DepartmentAddAsync(dataObj);
                        //}
                        //{
                        //  result = await _caller.BankAddAsync(dataObj);
                        //}

                        var jsonObj = JObject.Parse(result);
                        if ((int)jsonObj["status"] == 200)
                        {
                            var responseData = new Dictionary<string, string>
              {
                { "error", $"success" }
              };

                            return Ok(responseData);
                        }
                        else
                        {
                            errors.Add($"Department Add Error:{(string)jsonObj["title"]}");
                            return BadRequest(errors);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Department add failed due to {reason}", ex.GetBaseException().Message);
                        errors.Add($"Bank add failed");
                    }
                }
                else
                {
                    var modelErrors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Value.Errors }).ToList();
                    foreach (var errorItems in modelErrors)
                    {
                        foreach (var item in errorItems.Errors)
                        {
                            errors.Add(item.ErrorMessage);
                        }
                    }
                }
            }
            else
            {
                //in case of exists
                if (response == 3)
                {
                    errors.Add($"Department Name or Department Code already exists");
                }
                else
                {
                    errors.Add($"Department Name or Department Code already exists Deleted banks");
                }
            }

            //var jwt_token = HttpContext.Request.Cookies["jwt"].ToString();
            //_jwtClaims.GetUserClaims(jwt_token, out UserClaimVM dataObject);

            //*** method 1 / Array direct to Json
            //var json = JsonConvert.SerializeObject(errors);
            //return Content(json, "application/json");

            var jsonObj2 = new
            {
                errors = errors
            };
            return Json(jsonObj2);
            //}
        }



        //**************
        [HttpPost]
        public async Task<IActionResult> MyAction([FromHeader] BankAddEditVM dataObj)
        {
            // Do something with id
            return View();
        }

        [HttpPost]
        public IActionResult MyAction1(int id)
        {
            // Do something with id
            return View();
        }

    }
}
