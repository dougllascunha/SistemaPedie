using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using System.Text.Json;
using pimFinal.Models;

namespace pimFinal.Controllers;

public class CarrinhoController : Controller
{
    private readonly PedieDbContext _context;

    public CarrinhoController(PedieDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var carrinhoJson = HttpContext.Session.GetString("Carrinho");

        if (string.IsNullOrEmpty(carrinhoJson))
        {
            return View(new List<ItemCarrinho>());
        }

        var conversaoJson = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();

        // Transforma o JSON em objetos C# garantindo que não haja Produto nulo (NullReferenceException)
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
            .Where(item => item.Produto != null)
            .ToList();

        return View(itens);
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
            carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();
        }

        carrinho.Add(id);

        HttpContext.Session.SetString(
            "Carrinho",
            JsonSerializer.Serialize(carrinho));

        var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == id);
        string nomeProduto = produto?.Nome ?? "Item";

        // Se a requisição veio via AJAX (fetch), retorna JSON sem recarregar a página nem voltar ao topo
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            Request.Headers["Accept"].ToString().Contains("application/json"))
        {
            return Json(new
            {
                sucesso = true,
                nomeProduto = nomeProduto,
                totalItens = carrinho.Count,
                mensagem = $"\"{nomeProduto}\" foi adicionado ao carrinho!"
            });
        }

        TempData["Mensagem"] = $"\"{nomeProduto}\" foi adicionado ao carrinho!";

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction("Index", "Home");
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

        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();
        carrinho.RemoveAll(x => x == id);

        HttpContext.Session.SetString(
            "Carrinho",
            JsonSerializer.Serialize(carrinho));

        TempData["Mensagem"] = "Item removido do carrinho.";
        return RedirectToAction("Index");
    }
}
