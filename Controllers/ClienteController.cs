using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;

namespace pimFinal.Controllers
{
    public class ClienteController : Controller
    {
        private readonly PedieDbContext _context;

        public ClienteController(PedieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                TempData["Mensagem"] = "Conta criada com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            
            return View(cliente);
        }
    }
}