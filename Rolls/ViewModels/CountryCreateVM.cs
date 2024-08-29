using System.ComponentModel.DataAnnotations;

namespace Rolls.ViewModels
{
  public class CountryCreateVM
  {
    [DisplayName("Country Name")]
    [Required(ErrorMessage = "Country Name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Country Name should be 2 to 50 characters long.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country Name should contain only letters and spaces")]
    [Remote("CountryNameExists", "Country", HttpMethod = "POST", ErrorMessage = "Country Name already exits", AdditionalFields = "Country_id,Country")]
    public string country_name { get; set; }

    [DisplayName("Country Code")]
    [Required(ErrorMessage = "Country Code is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Country Code should be 3 to 5 characters long.")]
    [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Country Code should contain only letters")]
    [Remote("CountryCodeExists", "Country", HttpMethod = "POST", ErrorMessage = "Country Code already exits", AdditionalFields = "Country_id,Country")]
    public string country_code { get; set; }

    public string record_status { get; set; } = "ACTIVE";

    public IList<string>? errors { get; set; }

    public UserClaimVM? user_claims { get; set; }
  }
}
