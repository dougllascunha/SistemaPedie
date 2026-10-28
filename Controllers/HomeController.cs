using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
