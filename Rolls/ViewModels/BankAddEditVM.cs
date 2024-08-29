namespace Rolls.ViewModels
{
  public class BankAddEditVM
  {
    public long? bank_id { get; set; }

    [DisplayName("Bank Name")]
    //[Required(ErrorMessage = "Bank Name is requiredxxx")]
    //[StringLength(50, MinimumLength = 2, ErrorMessage = "Bank Name should be 2 to 50 characters long.")]
    //[RegularExpression(@"^[a-zA-Z\s\&]+$", ErrorMessage = "Bank Name should contain only letters and spaces")]
    //[Remote("BankNameExists", "Bank", HttpMethod = "POST", ErrorMessage = "Bank Name already exits")]
    public string bank_name { get; set; }

    //[StringLength(50, MinimumLength = 2, ErrorMessage = "Bank Name should be 2 to 50 characters long.''''")]
    //[RegularExpression(@"^[a-zA-Z\s\&]+$", ErrorMessage = "Bank Name should contain only letters and spaces''''")]
    public string? bank_name_edit { get; set; }


    public IList<string>? errors { get; set; }

    public UserClaimVM? user_claims { get; set; }

    public string? record_status { get; set; }

    public string? message { get; set; }
  }
}
