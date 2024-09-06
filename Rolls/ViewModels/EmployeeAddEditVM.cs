namespace Rolls.ViewModels
{
    public class EmployeeAddEditVM
    {
        public long? employee_id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "First Name must have atleast 2 letters")]
        public string first_name { get; set; }

        public string? first_name_edit { get; set; }

        [Required(ErrorMessage = "Cannot leave Middle Name blank")]
        [StringLength(20, ErrorMessage = "Middle Name cannot exceed 20 letters")]
        public string middle_name { get; set; }

        public string? middle_name_edit { get; set; }

        [Required(ErrorMessage = "Cannot leave Middle Name blank")]
        [StringLength(20, ErrorMessage = "Middle Name cannot exceed 20 letters")]
        public string last_name { get; set; }

        public string? last_name_edit { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email_id { get; set; }

        public string? email_id_edit { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please add a valid mobile number")]
        public string mobile {  get; set; }

        public string? mobile_edit { get; set; }

        [Required]
        public long department_id { get; set; }

        public long? department_id_edit { get; set; }

        [Required]
        public long designation_id { get; set; }

        public long? designation_id_edit { get; set; }

        public IList<string>? errors { get; set; }

        public UserClaimVM? user_claims { get; set; }

        public string? record_status { get; set; }

        public string? message { get; set; }

    }
}
