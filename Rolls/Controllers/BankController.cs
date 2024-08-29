using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Rolls.Controllers
{
	public class BankController : Controller
	{
		private readonly DbContext _dbContext;

		private readonly IBankCaller _caller;
		public BankController(IBankCaller caller)
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

			BankAddEditVM indexObj = new BankAddEditVM()
			{
				message = message
			};
			return View(indexObj);
		}

		[Route("/Bank/GetBanksAsync")]
		public async Task<JsonResult> GetBanksAsync()
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

			IList<Bank> dataObj = new List<Bank>();
			try
			{
				var result = await _caller.GetBanksAsync();

				var jsonObj = JObject.Parse(result);
				if ((int)jsonObj["status"] == 200)
				{
					var listObj = (dynamic)jsonObj["dataObj"];

					foreach (var item in listObj)
					{
						dataObj.Add(new Bank()
						{
							bank_id = item.bank_id,
							bank_name = item.bank_name
						});
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err, $"BankController/GetBanksAsync Err:{err.GetBaseException().Message}");
			}

			IEnumerable<Bank> bankObj = dataObj;
			if (!string.IsNullOrEmpty(searchValue))
			{
				bankObj = bankObj.Where(m => m.bank_name.Contains(searchValue));
			}
			if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
			{
				bankObj = bankObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
			}

			recordsTotal = bankObj.Count();
			var data = bankObj.Skip(skip).Take(pageSize).ToList();
			var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
		}

		[HttpPost("/Bank/AddBank")]
		public async Task<IActionResult> AddBank([FromBody] BankAddEditVM dataObj)
		{
			var response = 0;
			long bankId = 0;
			string bankName = dataObj.bank_name;
			if (dataObj.bank_id != null)
			{
				bankId = dataObj.bank_id.Value;
				bankName = dataObj.bank_name_edit;
			}
			IsExistsVM isExistsObj = new IsExistsVM() { str1 = bankName.ToUpper(), id = bankId.ToString() };
			var result1 = await _caller.BankNameExistsAsync(isExistsObj);
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
						var result = await _caller.BankAddAsync(dataObj);
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
							errors.Add($"Bank Add Error:{(string)jsonObj["title"]}");
							return BadRequest(errors);
						}
					}
					catch (Exception ex)
					{
						Log.Error(ex, "Bank add failed due to {reason}", ex.GetBaseException().Message);
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
					errors.Add($"Bank name already exists");
				}
				else
				{
					errors.Add($"Bank name already exists Deleted banks");
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


		[HttpPost("/Bank/EditBank")]
		//public async Task<IActionResult> EditBank(BankAddEditVM dataObj)
		public async Task<IActionResult> EditBank([FromBody] BankAddEditVM dataObj)
		{
			List<string> errors = new List<string>();

			//var jwt_token = HttpContext.Request.Cookies["jwt"].ToString();
			//_jwtClaims.GetUserClaims(jwt_token, out UserClaimVM dataObject);


			if (ModelState.IsValid)
			{
				try
				{
					//BankEditVM dataObj = new BankEditVM()
					//{
					//  bank_id = postData.bank_id,
					//  bank_name = postData.bank_name,


					//};
					//var result = await _caller.BankEditAsync(dataObj);


					//var jsonObj = JObject.Parse(result);
					//if ((int)jsonObj["status"] == 200)
					//{
					//  TempData["CrudStatus"] = "Record Edited";
					//  return RedirectToAction("Index", "Bank", new { id = postData.bank_id });
					//}
					//else
					//{
					//  errors.Add($"Bank edit Error:{(string)jsonObj["title"]}");
					//}
				}
				catch (Exception ex)
				{

					Log.Error(ex, "Bank edit failed due to {reason}", ex.GetBaseException().Message);
					errors.Add($"Bank edit failed");
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

			BankAddEditVM indexObj = null;
			//var response = await _caller.GetBankAsync(postData.bank_id);
			//var jsonObject = JObject.Parse(response);
			//if ((int)jsonObject["status"] == 200)
			//{
			//  var listObj = (dynamic)jsonObject["dataObj"];
			//  indexObj = new BankEditVM()
			//  {
			//    bank_id = listObj.bank_id,
			//    bank_name = listObj.bank_name,
			//    user_claims = dataObject,
			//    errors = errors
			//  };
			//}

			return View("Edit", indexObj);
		}

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


		[HttpPost("/Bank/DeleteBank")]
		public async Task<IActionResult> DeleteBank([FromBody] IdVM dataObj)
		{
			var response = 0;
			long bankId = 0;
		

			List<string> errors = new List<string>();
			if (ModelState.IsValid)
			{
				try
				{
	
					
					var result = await _caller.BankDeleteAsync(dataObj);
					

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
						errors.Add($"Bank Delete Error:{(string)jsonObj["title"]}");
						return BadRequest(errors);
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Bank Delete failed due to {reason}", ex.GetBaseException().Message);
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


		[Route("/Bank/GetDeletedBanksAsync")]
		public async Task<JsonResult> GetDeletedBanksAsync()
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

			IList<Bank> dataObj = new List<Bank>();
			try
			{
				var result = await _caller.GetDeletedBanksAsync();

				var jsonObj = JObject.Parse(result);
				if ((int)jsonObj["status"] == 200)
				{
					var listObj = (dynamic)jsonObj["dataObj"];

					foreach (var item in listObj)
					{
						dataObj.Add(new Bank()
						{
							bank_id = item.bank_id,
							bank_name = item.bank_name
						});
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err, $"BankController/GetDeletedBanksAsync Err:{err.GetBaseException().Message}");
			}

			IEnumerable<Bank> bankObj = dataObj;
			if (!string.IsNullOrEmpty(searchValue))
			{
				bankObj = bankObj.Where(m => m.bank_name.Contains(searchValue));
			}
			if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
			{
				bankObj = bankObj.AsQueryable().OrderBy(sortColumn + " " + sortColumnDirection);
			}

			recordsTotal = bankObj.Count();
			var data = bankObj.Skip(skip).Take(pageSize).ToList();
			var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
		}


        [HttpPost("/Bank/RestoreBank")]
        public async Task<IActionResult> RestoreBank([FromBody] IdVM dataObj)
        {
            var response = 0;
            long bankId = 0;


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {


                    var result = await _caller.BankRestoreAsync(dataObj);


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
                        errors.Add($"Bank Restore Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Bank Restore failed due to {reason}", ex.GetBaseException().Message);
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


        [HttpPost("/Bank/PermanentDelete")]
        public async Task<IActionResult> PermanentDelete([FromBody] IdVM dataObj)
        {
            var response = 0;
            long bankId = 0;


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {


                    var result = await _caller.BankPermanentDeleteAsync(dataObj);


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
                        errors.Add($"Bank Restore Error:{(string)jsonObj["title"]}");
                        return BadRequest(errors);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Bank Restore failed due to {reason}", ex.GetBaseException().Message);
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


    }
}
