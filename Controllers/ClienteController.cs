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

        // --- MÉTODOS DE LOGIN E LOGOUT ---
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senhaAcesso, string? returnUrl)
        {
            var cliente = _context.Clientes
                .FirstOrDefault(c => c.Email == email && c.SenhaAcesso == senhaAcesso);

            if (cliente != null)
            {
                HttpContext.Session.SetInt32("IdCliente", cliente.IdCliente);
                HttpContext.Session.SetString("NomeCliente", cliente.Nome);
                HttpContext.Session.SetString("Perfil", cliente.Perfil);
               
                TempData["Mensagem"] = $"Bem-vindo de volta, {cliente.Nome}!";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.Erro = "E-mail ou senha incorretos.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear(); // Apaga todos os dados do usuário logado
            return RedirectToAction("Index", "Home");
        }

        // --- MÉTODOS DE RECUPERAÇÃO DE SENHA ---
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarSenha(string email, string telefone)
        {
            // Validação simples (2FA Acadêmico): Confere se e-mail e telefone batem no banco
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == email && c.Telefone == telefone);
            
            if (cliente != null)
            {
                // Guarda o ID do cliente temporariamente para a próxima tela
                TempData["IdRecuperacao"] = cliente.IdCliente;
                return RedirectToAction("RedefinirSenha");
            }

            ViewBag.Erro = "Os dados informados não conferem com nenhum cadastro.";
            return View();
        }

        [HttpGet]
        public IActionResult RedefinirSenha()
        {
            // Se o usuário tentar acessar essa tela direto pela URL, é bloqueado
            if (TempData["IdRecuperacao"] == null) return RedirectToAction("Login");
            
            TempData.Keep("IdRecuperacao"); // Mantém o valor para o POST
            return View();
        }

        [HttpPost]
    
        public IActionResult RedefinirSenha(string novaSenha)
        {
            if (TempData["IdRecuperacao"] != null)
            {
                int idCliente = (int)TempData["IdRecuperacao"];
                var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == idCliente);
                
                if (cliente != null)
                {
                    cliente.SenhaAcesso = novaSenha; // Atualiza a senha
                    _context.SaveChanges();
                    
                    TempData["Mensagem"] = "Senha redefinida com sucesso! Faça seu login.";
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("Login");
        }
    }
}
