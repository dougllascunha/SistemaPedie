using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System.Linq;

namespace pimFinal.Controllers;

public class PromocaoController : Controller
{
    private readonly PedieDbContext _context;

    public PromocaoController(PedieDbContext context)
    {
        _context = context;
    }

    private bool IsAdmin() => HttpContext.Session.GetString("Perfil") == "Admin";

    // LISTAR PROMOÇÕES
    public IActionResult Index()
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        
        var promocoes = _context.Promocoes.OrderByDescending(p => p.DataInicio).ToList();
        return View(promocoes);
    }

    // CRIAR PROMOÇÃO (Traz a tela)
    public IActionResult Criar()
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public IActionResult Criar(Promocao promocao)
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");
        
        _context.Promocoes.Add(promocao);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // DELETAR PROMOÇÃO
    [HttpPost]
    public IActionResult Deletar(int idPromocao)
    {
        if (!IsAdmin()) return RedirectToAction("Index", "Home");

        var promocao = _context.Promocoes.FirstOrDefault(p => p.IdPromocao == idPromocao);
        if (promocao != null)
        {
            _context.Promocoes.Remove(promocao);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}

