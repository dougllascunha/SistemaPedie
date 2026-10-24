using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pimFinal.Models;
using pimFinal.Data;

namespace pimFinal.Controllers;

public class HomeController : Controller
{
    private readonly PedieDbContext _context;

        public HomeController(PedieDbContext context)
        {
            _context = context;
        }

    public IActionResult Index()

    {
        var produtos = _context.Produtos.ToList();
        return View(produtos);
    }

        public IActionResult TesteBanco()
        {
            var totalCategorias = _context.Categorias.Count();
            var produtos = _context.Produtos.Count();

            return Content($"Conexão OK! Categorias encontradas: {totalCategorias}|Quantidade de produtos{produtos}");
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
