using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudApplication.Models;

[Table("orders", Schema = "public")]
public class Order
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [Column("customer")]
    public string Customer { get; set; } = string.Empty;
    
    [Column("status")]
    public string Status { get; set; } = "open";
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Item> Items { get; set; } = new();
}

[Table("items", Schema = "public")]
public class Item
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [Column("order_id")]
    public string OrderId { get; set; } = string.Empty;
    
    [Required]
    [Column("sku")]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;
}