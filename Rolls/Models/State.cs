namespace Rolls.Models
{
  public class State
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long state_id { get; set; }

    [Required]
    [StringLength(50)]
    public string state_name { get; set; }

    [Required]
    [StringLength(50)]
    public string state_code { get; set; }

    [Required]
    [StringLength(50)]
    public string record_status { get; set; }

    [Required]
    public long country_id { get; set; }

    public virtual Country country { get; set; }
    public virtual ICollection<District> districts { get; set; }
  }
}
