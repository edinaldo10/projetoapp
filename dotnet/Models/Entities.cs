using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudApplication.Models;

[Table("orders", Schema = "public")]
public class Order
{
    [Key]
    [Column("id")]
    public string id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("customer")]
    public string customer { get; set; } = string.Empty;

    [Column("status")]
    public string status { get; set; } = "open";

    [Column("created_at")]
    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public List<Item> Items { get; set; } = new();
}

[Table("items", Schema = "public")]
public class Item
{
    [Key]
    [Column("id")]
    public string id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("order_id")]
    public string order_id { get; set; } = string.Empty;

    [Required]
    [Column("sku")]
    public string sku { get; set; } = string.Empty;

    [Required]
    [Column("description")]
    public string description { get; set; } = string.Empty;

    [Required]
    [Column("quantity")]
    public int quantity { get; set; }

    [ForeignKey("order_id")]
    public Order order { get; set; } = null!;
}


