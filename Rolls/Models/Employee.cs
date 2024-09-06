namespace Rolls.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long employee_id { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        public string middle_name { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        [Required]
        [StringLength(50)]
        public string email_id { get; set; }

        [Required]
        [StringLength(50)]
        public string mobile { get; set; }

        [Required]
        [StringLength(20)]
        public string record_status { get; set; }

        [Required]
        [StringLength(50)]
        public long department_id { get; set; }

        [Required]
        [StringLength(50)]
        public long designation_id { get; set; }
    }
}
