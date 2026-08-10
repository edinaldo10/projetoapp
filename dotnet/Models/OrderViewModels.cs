using System.ComponentModel.DataAnnotations;

namespace CloudApplication.Models;

public class OrderCreateViewModel
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Customer { get; set; } = string.Empty;
}

public class OrderUpdateViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Customer { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;
}

public class OrderDetailsViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ItemViewModel> Items { get; set; } = new();
}

public class ItemViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required] public string Sku { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantity { get; set; }
}