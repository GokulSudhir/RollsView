namespace Rolls.Models
{
  public class Bank
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long bank_id { get; set; }

    [Required]
    [StringLength(50)]
    public string bank_name { get; set; }

    [Required]
    [StringLength(50)]
    public string record_status { get; set; }
  }
}
