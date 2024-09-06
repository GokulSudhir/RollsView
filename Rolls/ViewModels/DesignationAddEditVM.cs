namespace Rolls.ViewModels
{
    public class DesignationAddEditVM
    {
        public long? designation_id { get; set; }

        [DisplayName("Designation Name")]
        public string designation_name { get; set; }
        public string? designation_name_edit { get; set; }

        [DisplayName("Designation Category")]
        public string designation_category { get; set; }

        public string? designation_category_edit { get; set; }

        public IList<string>? errors { get; set; }

        public UserClaimVM? user_claims { get; set; }

        public string? record_status { get; set; }

        public string? message { get; set; }
    }
}
