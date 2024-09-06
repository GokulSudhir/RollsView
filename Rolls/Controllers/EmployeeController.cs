using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rolls.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeCaller _caller;

        public EmployeeController(IEmployeeCaller caller)
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

            EmployeeAddEditVM indexObj = new EmployeeAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }

        [HttpPost("/Employee/AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeAddEditVM dataObj)
        {
            var response = 0;
            long employeeId = 0;
            int statusCode = 401;

            string firstName = dataObj.first_name;
            string lastName = dataObj.last_name;
            string middleName = dataObj.middle_name;
            string emailId = dataObj.email_id;
            string mobilE = dataObj.mobile;
            long? departmentId = dataObj.department_id;
            long? designationId = dataObj.designation_id;

            if (dataObj.employee_id != null)
            {
                employeeId = dataObj.employee_id.Value;
                firstName = dataObj.first_name_edit;
                middleName = dataObj.middle_name_edit;
                lastName = dataObj.last_name_edit;
                emailId = dataObj.email_id_edit;
                mobilE = dataObj.mobile_edit;
                departmentId = dataObj.department_id_edit;
                designationId = dataObj.designation_id_edit;
            }
            DoesEmployeeExist isExistsObj = new DoesEmployeeExist() { mobile = mobilE, id = employeeId.ToString() };
            var result1 = await _caller.EmployeeExistsAsync(isExistsObj);

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
                        dataObj.record_status = "ACTIVE";

                        var result = await _caller.EmployeeAddAsync(dataObj);

                        var jsonObj = JObject.Parse(result);

                        if ((int)jsonObj["status"] == 200)
                        {
                            statusCode = 200;

                        }
                        else
                        {
                            errors.Add($"Employee Add Error:{(string)jsonObj["title"]}");
                            return BadRequest(errors);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Employee add failed due to {reason}", ex.GetBaseException().Message);
                        errors.Add($"Employee add failed");
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
                    errors.Add($" Employee already exists");
                }
                else
                {
                    errors.Add($"Employee already exists in Deleted Employees");
                }
            }

            var jsonObj2 = new
            {
                errors = errors,
                statusCode = statusCode
            };

            return Json(jsonObj2);
        }

        [Route("/Employee/GetEmployees")]
        public async Task<JsonResult> GetEmployees()
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

            IList<EmployeeGet> dataObj = new List<EmployeeGet>();
            try
            {
                var result = await _caller.GetEmployeesAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new EmployeeGet()
                        {
                            employee_id = item.employee_id,
                            first_name = item.first_name,
                            middle_name = item.middle_name,
                            last_name = item.last_name,
                            email_id = item.email_id,
                            mobile = item.mobile,
                            record_status = item.record_status,
                            department_id = item.department_id,
                            designation_id = item.designation_id,
                            designation_name = item.designation_name,
                            department_name = item.department_name
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"EmployeeController/GetEmployees Err:{err.GetBaseException().Message}");
            }

            IEnumerable<EmployeeGet> employeeObj = dataObj;
            if (!string.IsNullOrEmpty(searchValue))
            {
                employeeObj = employeeObj.Where(m => m.first_name.Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                employeeObj = employeeObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
            }

            recordsTotal = employeeObj.Count();
            var data = employeeObj.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        [HttpPost("/Employee/DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee([FromBody] EmployeeDeleteVM dataObj)
        {
            int statusCode = 401;

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.EmployeeDeleteAsync(dataObj);

                    var jsonObj = JObject.Parse(result);
                    if ((int)jsonObj["status"] == 200)
                    {
                        statusCode = 200;
                    }
                    else
                    {
                        errors.Add($"Employee Delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Employee Delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Employee Delete failed");
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

            var jsonObj2 = new
            {
                errors = errors,
                statusCode = statusCode
            };
            return Json(jsonObj2);

        }

        [Route("/Employee/DeletedEmployees")]
        public async Task<JsonResult> GetDeletedEmployees()
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

            IList<EmployeeGet> dataObj = new List<EmployeeGet>();
            try
            {
                var result = await _caller.GetDeletedEmployeesAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new EmployeeGet()
                        {
                            employee_id = item.employee_id,
                            first_name = item.first_name,
                            middle_name = item.middle_name,
                            last_name = item.last_name,
                            email_id = item.email_id,
                            mobile = item.mobile,
                            record_status = item.record_status,
                            department_id = item.department_id,
                            designation_id = item.designation_id,
                            designation_name = item.designation_name,
                            department_name = item.department_name
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"EmployeeController/GetDeletedEmployees Err:{err.GetBaseException().Message}");
            }

            IEnumerable<EmployeeGet> employeeObj = dataObj;
            if (!string.IsNullOrEmpty(searchValue))
            {
                employeeObj = employeeObj.Where(m => m.first_name.Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                employeeObj = employeeObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
            }

            recordsTotal = employeeObj.Count();
            var data = employeeObj.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        [HttpPost("/Employee/RestoreEmployee")]
        public async Task<IActionResult> RestoreEmployee([FromBody] EmployeeDeleteVM dataObj)
        {
            int statusCode = 401;
            List<string> errors = new List<string>();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.EmployeeRestoreAsync(dataObj);

                    var jsonObj = JObject.Parse(result);

                    if ((int)jsonObj["status"] == 200)
                    {
                        statusCode = 200;
                    }
                    else
                    {
                        errors.Add($"Employee Restore Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Employee Restore failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Employee Restore failed");
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

            var jsonObj2 = new
            {
                errors = errors,
                statusCode = statusCode
            };
            return Json(jsonObj2);

        }

        [HttpPost("/Employee/PermanentDelete")]
        public async Task<IActionResult> PermanentDelete([FromBody] EmployeeDeleteVM dataObj)
        {
            int statusCode = 401;

            List<string> errors = new List<string>();

            if (ModelState.IsValid)
            {
                try
                {

                    var result = await _caller.EmployeePermanentDeleteAsync(dataObj);

                    var jsonObj = JObject.Parse(result);

                    if ((int)jsonObj["status"] == 200)
                    {
                        statusCode = 200;
                    }
                    else
                    {
                        errors.Add($"Employee delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Employee delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Employee Restore failed");
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

            var jsonObj2 = new
            {
                errors = errors,
                statusCode = statusCode
            };
            return Json(jsonObj2);

        }

    }
}
