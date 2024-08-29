namespace Rolls.Models
{
  public class District
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long district_id { get; set; }

    [Required]
    [StringLength(50)]
    public string district_name { get; set; }

    [Required]
    [StringLength(5)]
    public string district_code { get; set; }

    [Required]
    [StringLength(50)]
    public string record_status { get; set; }

    public long created_by { get; set; }

    [Required]
    [Column(TypeName = "timestamp")]
    public DateTime created_on { get; set; }

    public long updated_by { get; set; }

    [Required]
    [Column(TypeName = "timestamp")]
    public DateTime updated_on { get; set; }

    [Required]
    public long state_id { get; set; }

    public virtual State state { get; set; }
  }
}
