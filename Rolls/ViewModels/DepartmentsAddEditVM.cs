namespace Rolls.ViewModels
{
    public class DepartmentsAddEditVM
    {
        public long? department_id { get; set; }

        [DisplayName("Department Name")]
        public string department_name { get; set; }
        public string? department_name_edit { get; set; }

        [DisplayName("Department Classification")]
        public string department_classification { get; set; }

        public string? department_classification_edit { get; set; }

        public IList<string>? errors { get; set; }

        public UserClaimVM? user_claims { get; set; }

        public string? record_status { get; set; }

        public string? message { get; set; }
    }
}
