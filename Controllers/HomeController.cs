using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pimFinal.Models;
using pimFinal.Data;
using System.Linq;

namespace pimFinal.Controllers;

public class HomeController : Controller
{
    private readonly PedieDbContext _context;

    public HomeController(PedieDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string busca, int? idCategoria)
    {
        ViewBag.Categorias = _context.Categorias.ToList();
        ViewBag.BuscaAtual = busca;
        ViewBag.CategoriaAtual = idCategoria;

        // apenas produtos disponíveis
        var query = _context.Produtos.Where(p => p.Disponivel).AsQueryable();

        if (idCategoria.HasValue)
        {
            query = query.Where(p => p.IdCategoria == idCategoria.Value);
        }

        if (!string.IsNullOrEmpty(busca))
        {
            query = query.Where(p => p.Nome.Contains(busca));
        }

        var produtos = query.ToList();
        return View(produtos);
    }

    public IActionResult Promocoes()
    {
        // Puxa apenas as promoções que estão dentro do prazo de validade
        var promocoesAtivas = _context.Promocoes
            .Where(p => p.DataInicio <= System.DateTime.Now && p.DataFim >= System.DateTime.Now)
            .ToList();

        return View(promocoesAtivas);
    }

    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult SincronizarDadosMobile()
    {
        var carrinhoJson = HttpContext.Session.GetString("Carrinho");
        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson ?? "[]") ?? new List<int>();

        var produtos = _context.Produtos
            .AsNoTracking()
            .Select(p => new
            {
                idProduto = p.IdProduto,
                nome = p.Nome,
                descricao = p.Descricao,
                precoFormatado = p.Preco.ToString("N2"),
                disponivel = p.Disponivel,
                imagemUrl = p.ImagemUrl
            })
            .ToList();

        return Json(new
        {
            timestamp = System.DateTime.Now.ToString("HH:mm:ss"),
            totalItensCarrinho = carrinho.Count,
            produtos
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
