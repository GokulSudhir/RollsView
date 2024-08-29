namespace Rolls.Models
{
  public class Country
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long country_id { get; set; }

    [Required]
    [StringLength(50)]
    public string country_name { get; set; }

    [Required]
    [StringLength(5)]
    public string country_code { get; set; }

    [Required]
    [StringLength(50)]
    public string record_status { get; set; }

    public virtual ICollection<State> states { get; set; }
  }
}
