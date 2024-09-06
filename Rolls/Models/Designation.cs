namespace Rolls.Models
{
    public class Designation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long designation_id { get; set; }

        [Required]
        [StringLength(50)]
        public string designation_name { get; set; }

        [Required]
        [StringLength(50)]
        public string designation_category { get; set; }

        [Required]
        [StringLength(50)]
        public string record_status { get; set; }
    }
}
