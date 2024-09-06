using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rolls.Models;

namespace Rolls.Controllers
{
    public class DesignationController : Controller
    {
        private readonly DbContext _dbContext;
        private readonly IDesignationCaller _caller;

        public DesignationController(IDesignationCaller caller)
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

            DesignationAddEditVM indexObj = new DesignationAddEditVM()
            {
                message = message
            };
            return View(indexObj);
        }

        [Route("/Designation/GetDesignations")]
        public async Task<JsonResult> GetDesignations()
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

            IList<Designation> dataObj = new List<Designation>();
            try
            {
                var result = await _caller.GetDesignationsAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new Designation()
                        {
                            designation_id = item.designation_id,
                            designation_name = item.designation_name,
                            designation_category = item.designation_category
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DesignationController/GetDesignations Err:{err.GetBaseException().Message}");
            }

            IEnumerable<Designation> designationObj = dataObj;
            if (!string.IsNullOrEmpty(searchValue))
            {
                designationObj = designationObj.Where(m => m.designation_name.Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                designationObj = designationObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
            }

            recordsTotal = designationObj.Count();
            var data = designationObj.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }


        [HttpPost("/Designation/AddDesignation")]
        public async Task<IActionResult> AddDesignationAsync([FromBody] DesignationAddEditVM dataObj)
        {
            var response = 0;
            long designationId = 0;
            string designationName = dataObj.designation_name;
            string designationCategory = dataObj.designation_category;
            if (dataObj.designation_id != null)
            {
                designationId = dataObj.designation_id.Value;
                designationName = dataObj.designation_name_edit;
                designationCategory = dataObj.designation_category_edit;

            }
            IsExistsVM isExistsObj = new IsExistsVM() { str1 = designationName, str2 = designationCategory, id = designationId.ToString() };
            var result1 = await _caller.DesignationNameExistsAsync(isExistsObj);

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
                        var result = await _caller.DesignationAddAsync(dataObj);

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
                            errors.Add($"Designation Add Error:{(string)jsonObj["title"]}");
                            return BadRequest(errors);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Designation add failed due to {reason}", ex.GetBaseException().Message);
                        errors.Add($"Designation add failed");
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
                    errors.Add($"Designation Name or Designation Category already exists");
                }
                else
                {
                    errors.Add($"Designation Name or Designation Category already exists in Deleted Designations");
                }
            }

            var jsonObj2 = new
            {
                errors = errors
            };
            return Json(jsonObj2);
        }


        [HttpPost("/Designation/DeleteDesignation")]
        public async Task<IActionResult> DeleteDesignation([FromBody] DesignationDeleteVM dataObj)
        {
            var response = 0;
            long departmentsId = 0;


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.DesignationDeleteAsync(dataObj);

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
                        errors.Add($"Designation Delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Designation Delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Designation Delete failed");
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


        [Route("/Designation/DeletedDesignations")]
        public async Task<JsonResult> GetDeletedDesignations()
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

            IList<Designation> dataObj = new List<Designation>();
            try
            {
                var result = await _caller.GetDeletedDesignationsAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new Designation()
                        {
                            designation_id = item.designation_id,
                            designation_name = item.designation_name,
                            designation_category = item.designation_category
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DesignationController/GetDeletedDesignations Err:{err.GetBaseException().Message}");
            }

            IEnumerable<Designation> designationObj = dataObj;
            if (!string.IsNullOrEmpty(searchValue))
            {
                designationObj = designationObj.Where(m => m.designation_name.Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                designationObj = designationObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
            }

            recordsTotal = designationObj.Count();
            var data = designationObj.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }


        [HttpPost("/Designation/RestoreDesignation")]
        public async Task<IActionResult> RestoreDesignation([FromBody] DesignationDeleteVM dataObj)
        {

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _caller.DesignationRestoreAsync(dataObj);


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
                        errors.Add($"Designation Restore Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Designation Restore failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Designation Restore failed");
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


        [HttpPost("/Designation/PermanentDelete")]
        public async Task<IActionResult> PermanentDelete([FromBody] DesignationDeleteVM dataObj)
        {

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {

                    var result = await _caller.DesignationPermanentDeleteAsync(dataObj);

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
                        errors.Add($"Designation delete Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Designation delete failed due to {reason}", ex.GetBaseException().Message);
                    errors.Add($"Designation Restore failed");
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


        [Route("/Designation/DesignationDropDown")]
        public async Task<JsonResult> DesignationDropDown()
        {
            List<DesignationDropDown> dataObj = new List<DesignationDropDown>();
            try
            {
                var result = await _caller.DesignationDropDownAsync();

                var jsonObj = JObject.Parse(result);
                if ((int)jsonObj["status"] == 200)
                {
                    var listObj = (dynamic)jsonObj["dataObj"];

                    foreach (var item in listObj)
                    {
                        dataObj.Add(new DesignationDropDown()
                        {
                            designation_id = item.designation_id,
                            designation_name = item.designation_name
                        });
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error(err, $"DesignationController/DesignationDropDown Err:{err.GetBaseException().Message}");
            }

            return Json(dataObj);
        }
    }
}
