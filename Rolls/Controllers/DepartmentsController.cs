using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Rolls.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly IDepartmentsCaller _caller;

        public DepartmentsController(IDepartmentsCaller caller)
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

            DepartmentsAddEditVM indexObj = new DepartmentsAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }

        [Route("/Departments/GetDepartmentsAsync")]
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

            IList<Departments> dataObj = new List<Departments>();
            try
            {
                var result = await _caller.GetDepartmentsAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new Departments()
                        {
                            department_id = item.department_id,
                            department_name = item.department_name,
                            department_classification = item.department_classification
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DepartmentController/GetDepartmentssAsync Err:{err.GetBaseException().Message}");
            }

            IEnumerable<Departments> departmentObj = dataObj;
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

        [HttpPost("/Departments/AddDepartment")]
        public async Task<IActionResult> AddDepartmentAsync([FromBody] DepartmentsAddEditVM dataObj)
        {
            var response = 0;
            long departmentId = 0;
            string departmentName = dataObj.department_name;
            string departmentClassification = dataObj.department_classification;
            if (dataObj.department_id != null)
            {
                departmentId = dataObj.department_id.Value;
                departmentName = dataObj.department_name_edit;
                departmentClassification = dataObj.department_classification_edit;

            }
            IsExistsVM isExistsObj = new IsExistsVM() { str1 = departmentName, str2 = departmentClassification, id = departmentId.ToString() };
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
                        dataObj.record_status = "ACTIVE";
                        var result = await _caller.DepartmentAddAsync(dataObj);

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
                    errors.Add($"Department Name or Department Classification already exists");
                }
                else
                {
                    errors.Add($"Department Name or Department Classification already exists in Deleted Departments");
                }
            }

            var jsonObj2 = new
            {
                errors = errors
            };
            return Json(jsonObj2);
        }

        [HttpPost("/Departments/DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartmentsAsync([FromBody] DepartmentsDeleteVM dataObj)
        {
            var response = 0;
            long departmentsId = 0;


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.DepartmentDeleteAsync(dataObj);

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
                        errors.Add($"Department Delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Department Delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Bank Delete failed");
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
                errors = errors
            };
            return Json(jsonObj2);

        }

        [Route("/Departments/GetDeletedDepartments")]
        public async Task<JsonResult> GetDeletedDepartmentsAsync()
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

            IList<Departments> dataObj = new List<Departments>();
            try
            {
                var result = await _caller.GetDeletedDepartmentsAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new Departments()
                        {
                            department_id = item.department_id,
                            department_name = item.department_name,
                            department_classification = item.department_classification
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DepartmentsController/GetDeletedDepartments Err:{err.GetBaseException().Message}");
            }

            IEnumerable<Departments> departmentObj = dataObj;
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

        [HttpPost("/Departments/RestoreDepartment")]
        public async Task<IActionResult> RestoreBank([FromBody] DepartmentsDeleteVM dataObj)
        { 

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.DepartmentRestoreAsync(dataObj);


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
                        errors.Add($"Department Restore Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Department Restore failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Department Restore failed");
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
                errors = errors
            };
            return Json(jsonObj2);

        }

        [HttpPost("/Departments/PermanentDelete")]
        public async Task<IActionResult> PermanentDelete([FromBody] DepartmentsDeleteVM dataObj)
        {

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {

                    var result = await _caller.DepartmentPermanentDeleteAsync(dataObj);

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
                        errors.Add($"Department delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Department delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Bank Restore failed");
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
                errors = errors
            };
            return Json(jsonObj2);

        }

        [Route("/Departments/DepartmentDropDown")]
        public async Task<JsonResult> DepartmentDropDown()
        {
            List<DepartmentDropDown> dataObj = new List<DepartmentDropDown>();
            try
            {
                var result = await _caller.DepartmentDropDownAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new DepartmentDropDown()
                        {
                            department_id = item.department_id,
                            department_name = item.department_name
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DepartmentsController/DepartmentDropDown Err:{err.GetBaseException().Message}");
            }

            return Json(dataObj);
        }
    }
}
