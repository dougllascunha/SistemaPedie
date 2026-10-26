using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace pimFinal.Controllers;

public class PedidoController : Controller
{
    private readonly PedieDbContext _context;

    public PedidoController(PedieDbContext context)
    {
        _context = context;
    }
    [HttpPost]
public IActionResult FinalizarCompra()
{
        var idCliente = HttpContext.Session.GetInt32("IdCliente");

        if (idCliente == null)
            {
                return RedirectToAction("Login", "Cliente");
            }

        var carrinhoJson = HttpContext.Session.GetString("Carrinho");

        if (string.IsNullOrEmpty(carrinhoJson))
            {
                return RedirectToAction("Index", "Carrinho");
            }

        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson)
                    ?? new List<int>();

        var pedido = new Pedido
            {
                IdCliente = idCliente.Value,
                DataPedido = DateTime.Now,
                Total = 0
            };

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();

        decimal totalPedido = 0;

        var grupos = carrinho.GroupBy(id => id);

        foreach (var grupo in grupos)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == grupo.Key);

            if (produto == null)
                continue;

            var itemPedido = new ItemPedido
                {
                    IdPedido = pedido.IdPedido,
                    IdProduto = produto.IdProduto,
                    Quantidade = grupo.Count(),
                    PrecoUnitario = produto.Preco
                };

            _context.ItensPedido.Add(itemPedido);

            totalPedido += produto.Preco * grupo.Count();
        }

        pedido.Total = totalPedido;

        _context.SaveChanges();

        HttpContext.Session.Remove("Carrinho");

        return RedirectToAction("Sucesso");
    }
    public IActionResult Sucesso()
    {
        return View();
    }

}