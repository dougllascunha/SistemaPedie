using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using System.Text.Json;
using pimFinal.Models;

namespace pimFinal.Controllers;

public class CarrinhoController : Controller
{
   public IActionResult Index()
{
    var carrinhoJson = HttpContext.Session.GetString("Carrinho");

    if (string.IsNullOrEmpty(carrinhoJson))
    {
        return View(new List<ItemCarrinho>());
    }

    var conversaoJson = JsonSerializer.Deserialize<List<int>>(carrinhoJson)?? new List<int>();
    //transforma o Json em objeto c#
    var itens = conversaoJson
        .GroupBy(id => id)
        .Select(grupo =>
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == grupo.Key);

            return new ItemCarrinho
            {
                Produto = produto,
                Quantidade = grupo.Count()
            };
        })
        .ToList();

    return View(itens);
    }

    private readonly PedieDbContext _context;
    public CarrinhoController(PedieDbContext context)
    {
        _context = context;
    }
    public IActionResult Adicionar(int id)
    {
        var carrinhoJson = HttpContext.Session.GetString("Carrinho");

        List<int> carrinho;

        if (string.IsNullOrEmpty(carrinhoJson))
            {
                carrinho = new List<int>();
            }
        else
            {
                carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson)??new List<int>();
            }

        carrinho.Add(id);

        HttpContext.Session.SetString(
            "Carrinho",
            JsonSerializer.Serialize(carrinho));

        return Redirect(Request.Headers["Referer"].ToString());
    }
    public IActionResult Aumentar(int id)
    {
        var carrinhoJson = HttpContext.Session.GetString("Carrinho");

        if (string.IsNullOrEmpty(carrinhoJson))
        {
            return RedirectToAction("Index");
        }

        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();

        carrinho.Add(id);

        HttpContext.Session.SetString(
            "Carrinho",
            JsonSerializer.Serialize(carrinho));

        return RedirectToAction("Index");
    }
    public IActionResult Diminuir(int id)
    {
        var carrinhoJson = HttpContext.Session.GetString("Carrinho");

        if (string.IsNullOrEmpty(carrinhoJson))
        {
            return RedirectToAction("Index");
        }

        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();

        carrinho.Remove(id);

        HttpContext.Session.SetString(
            "Carrinho",
            JsonSerializer.Serialize(carrinho));

        return RedirectToAction("Index");
    }
    public IActionResult Remover(int id)
{
    var carrinhoJson = HttpContext.Session.GetString("Carrinho");

    if (string.IsNullOrEmpty(carrinhoJson))
    {
        return RedirectToAction("Index");
    }

    var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson)
                   ?? new List<int>();

    carrinho.RemoveAll(x => x == id);

    HttpContext.Session.SetString(
        "Carrinho",
        JsonSerializer.Serialize(carrinho));

    return RedirectToAction("Index");
}
    

   
}