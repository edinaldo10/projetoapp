using CloudApplication.Data;
using CloudApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudApplication.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.created_at)
                .Select(o => new OrderDetailsViewModel
                {
                    id = o.id,
                    customer = o.customer,
                    status = o.status,
                    created_at = o.created_at,
                    Items = o.Items.Select(i => new ItemViewModel
                    {
                        id = i.id,
                        sku = i.sku,
                        description = i.description,
                        quantity = i.quantity
                    }).ToList()
                })
                .ToListAsync();

            return View(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos.");
            TempData["ErrorMessage"] = "Erro ao carregar os pedidos.";
            return View(new List<OrderDetailsViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string customer)
    {
        if (string.IsNullOrWhiteSpace(customer))
        {
            TempData["ErrorMessage"] = "O nome do cliente é obrigatório.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var order = new Order
            {
                customer = customer,
                status = "Created",
                created_at = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Pedido criado com sucesso!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido.");
            TempData["ErrorMessage"] = "Erro interno ao salvar o pedido.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var order = await _context.Orders.FirstOrDefaultAsync(o => EF.Property<string>(o, "id") == id);
        if (order == null) return NotFound();

        var model = new OrderUpdateViewModel
        {
            id = order.id,
            customer = order.customer,
            status = order.status
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, OrderUpdateViewModel model)
    {
        if (id != model.id) return NotFound();

        if (!ModelState.IsValid) return View(model);

        try
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => EF.Property<string>(o, "id") == id);
            if (order == null) return NotFound();

            order.customer = model.customer;
            order.status = model.status;

            _context.Update(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Pedido atualizado com sucesso!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar o pedido {OrderId}", id);
            TempData["ErrorMessage"] = "Erro ao atualizar o pedido.";
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => EF.Property<string>(o, "id") == id);

            if (order != null)
            {
                _context.Items.RemoveRange(order.Items);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pedido excluído com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Pedido não encontrado.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir o pedido {OrderId}", id);
            TempData["ErrorMessage"] = "Erro ao excluir o pedido.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddItem(string orderId, string sku, string description, int quantity)
    {
        try
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => EF.Property<string>(o, "id") == orderId);
            if (order == null) return NotFound();

            var item = new Item
            {
                order_id = orderId,
                sku = sku,
                description = description,
                quantity = quantity
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item adicionado com sucesso!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar item ao pedido {OrderId}", orderId);
            TempData["ErrorMessage"] = "Erro ao adicionar item.";
        }

        return RedirectToAction(nameof(Index));
    }
}