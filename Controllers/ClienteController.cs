using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System;
using System.Linq; 

namespace pimFinal.Controllers
{
    public class ClienteController : Controller
    {
        private readonly PedieDbContext _context;

        public ClienteController(PedieDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS DE CADASTRO ---
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
                cliente.Perfil = "Cliente";
                cliente.DataCadastro = DateTime.Now;

                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                TempData["Mensagem"] = "Conta criada com sucesso! Faça seu login.";
                return RedirectToAction("Login");
            }
            return View(cliente);
        }

        // --- MÉTODOS DE LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senhaAcesso)
        {
            
            var cliente = _context.Clientes
                .FirstOrDefault(c => c.Email == email && c.SenhaAcesso == senhaAcesso);

            if (cliente != null)
            {
                HttpContext.Session.SetInt32(
                "Id Cliente",
                cliente.IdCliente);

                HttpContext.Session.SetString(
                "NomeCliente",
                cliente.Nome);
               
                TempData["Mensagem"] = $"Bem-vindo de volta, {cliente.Nome}!";
                return RedirectToAction("Index", "Home");
            }

            
            ViewBag.Erro = "E-mail ou senha incorretos.";
            return View();
        }
    }
}
