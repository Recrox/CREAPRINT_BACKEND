namespace CreaPrintCore.Models
{
 public abstract class Item : BaseItem
 {
 public string? CreatedBy { get; set; }
 public DateTime CreatedOn { get; set; }
 public string? UpdatedBy { get; set; }
 public DateTime? UpdatedOn { get; set; }
 }
}
