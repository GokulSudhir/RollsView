using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
//using Rolls.ViewModels;
using System.Drawing.Printing;

using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.IO.Font;
using iText.Pdfa;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

namespace Rolls.Controllers
{

  public class CountryController : Controller
  {
    //private readonly IJwtClaims _jwtClaims;
    private readonly ICountryCaller _caller;
    public CountryController(ICountryCaller caller)
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

      IndexVM indexObj = new IndexVM()
      {
        message = message
      };
      return View(indexObj);
    }


    [Route("/Country/GetCountriesAsync")]
    public async Task<JsonResult> GetCountriesAsync()
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

      IList<Country> dataObj = new List<Country>();
      try
      {
        var result = await _caller.GetCountriesAsync();

        var jsonObj = JObject.Parse(result);
        if ((int)jsonObj["status"] == 200)
        {
          var listObj = (dynamic)jsonObj["dataObj"];

          foreach (var item in listObj)
          {
            dataObj.Add(new Country()
            {
              country_id = item.country_id,
              country_code = item.country_code,
              country_name = item.country_name
            });
          }
        }
      }
      catch (Exception err)
      {
        Log.Error(err, $"CountryController/GetCountriesAsync Err:{err.GetBaseException().Message}");
      }

      IEnumerable<Country> bankObj = dataObj;
      if (!string.IsNullOrEmpty(searchValue))
      {
        bankObj = bankObj.Where(m => m.country_code.Contains(searchValue));
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

    public IActionResult Create()
    {
      var jwt_token = HttpContext.Request.Cookies["jwt"].ToString();

      //_jwtClaims.GetUserClaims(jwt_token, out UserClaimVM dataObj);

      IList<string> errors = new List<string>();

      CountryCreateVM indexObj = new CountryCreateVM()
      {
        //user_claims = dataObj,
        errors = errors
      };
      return View(indexObj);
    }


    [HttpGet]
    public async Task<IActionResult> IsCountryExistsAsync(string input,  AdditionalFieldsVM additionalFields)
    {

      var response = 0;
      try
      {
        IsExistsVM isExistsObj = new IsExistsVM() { str1 = input, id = additionalFields.Field1 };

        var result = await _caller.CountryNameExistsAsync(isExistsObj);
        var jsonObj = JObject.Parse(result);
        if ((int)jsonObj["status"] == 200)
        {
          var dataObj = (dynamic)jsonObj["dataObj"];
          if (dataObj == null)
          {
            response = 1;
          }
          else
          {
            if (dataObj.record_status == "DELETED")
            {
              response = 2;
            }
            else
            {
              response = 3;
            }
          }
        }

      }
      catch (Exception err)
      {
        response = 1;
        Log.Error(err, $"BankController/BankNameExists Err:{err.GetBaseException().Message}");
      }

      //switch (response)
      //{
      //  case 1:
      //    return Json(response);

      //  case 2:
      //    return Json("Bank name already exists in deleted list");

      //  case 3:
      //    return Json("Bank name already exists");
      //  default:
      //    return Json(true);
      //}

      return Json(new { isValid = true });

      //bool isValid = false; /* perform validation logic */
      //return Json(isValid);
    }

  }
}
