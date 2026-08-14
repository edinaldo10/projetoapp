using System.ComponentModel.DataAnnotations;

namespace CloudApplication.Models;

public class OrderCreateViewModel
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string customer { get; set; } = string.Empty;
}

public class OrderUpdateViewModel
{
    [Required]
    public string id { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string customer { get; set; } = string.Empty;

    [Required]
    public string status { get; set; } = string.Empty;
}

public class OrderDetailsViewModel
{
    public string id { get; set; } = string.Empty;
    public string customer { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public List<ItemViewModel> Items { get; set; } = new();
}

public class ItemViewModel
{
    public string id { get; set; } = string.Empty;

    [Required]
    public string sku { get; set; } = string.Empty;

    [Required]
    public string description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int quantity { get; set; }
}