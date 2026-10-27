using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System.Linq;

namespace pimFinal.Controllers;

public class ProdutoController : Controller
{
    private readonly PedieDbContext _context;

    public ProdutoController(PedieDbContext context)
    {
        _context = context;
    }

    
    private bool IsAdmin() => HttpContext.Session.GetString("Perfil") == "Admin";

    
    public IActionResult Index()
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        var produtos = _context.Produtos.ToList();
        return View(produtos);
    }

    
    public IActionResult Criar()
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        ViewBag.Categorias = _context.Categorias.ToList();
        return View();
    }

    
    [HttpPost]
    public IActionResult Criar(Produto produto)
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        
        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    
    [HttpPost]
    public IActionResult Deletar(int idProduto)
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");

        var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == idProduto);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
