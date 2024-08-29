namespace Rolls.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long department_id { get; set; }

        [Required]
        [StringLength(50)]
        public string department_name { get; set; }

        [Required]
        [StringLength(50)]
        public string department_code { get; set; }

        [Required]
        [StringLength(50)]
        public string record_status { get; set; }
    }
}
