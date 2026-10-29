using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace pimFinal.Controllers
{
    public class ClienteController : Controller
    {
        private readonly PedieDbContext _context;

        public ClienteController(PedieDbContext context)
        {
            _context = context;
        }

        private bool CarrinhoTemItens()
        {
            var carrinhoJson = HttpContext.Session.GetString("Carrinho");
            if (string.IsNullOrEmpty(carrinhoJson)) return false;
            var itens = JsonSerializer.Deserialize<List<int>>(carrinhoJson);
            return itens != null && itens.Count > 0;
        }

        // --- MÉTODOS DE CADASTRO ---
        [HttpGet]
        public IActionResult Cadastrar(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Cliente cliente, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (_context.Clientes.Any(c => c.Email == cliente.Email))
            {
                ViewBag.Erro = "Este e-mail já está cadastrado. Faça login ou recupere sua senha.";
                return View(cliente);
            }

            if (ModelState.IsValid)
            {
                cliente.Perfil = "Cliente";
                cliente.DataCadastro = DateTime.Now;

                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                TempData["Mensagem"] = "Conta criada com sucesso! Faça seu login para continuar.";
                return RedirectToAction("Login", new { returnUrl });
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
                string nomeExibicao = !string.IsNullOrWhiteSpace(cliente.NomeSocial) ? cliente.NomeSocial : cliente.Nome;

                HttpContext.Session.SetInt32("IdCliente", cliente.IdCliente);
                HttpContext.Session.SetString("NomeCliente", nomeExibicao);
                HttpContext.Session.SetString("Perfil", cliente.Perfil);

                TempData["Mensagem"] = $"Bem-vindo(a) de volta, {nomeExibicao}!";

                // Se houver itens no carrinho e o returnUrl for vazio ou apontar para a Home,
                // redireciona o cliente diretamente de volta para o Carrinho para concluir a compra.
                if (CarrinhoTemItens() &&
                    (string.IsNullOrEmpty(returnUrl) ||
                     returnUrl == "/" ||
                     returnUrl.StartsWith("/Home", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index", "Carrinho");
                }

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                {
                    return LocalRedirect(returnUrl);
                }

                return Redirect("/?auth=1");
            }

            ViewBag.Erro = "E-mail ou senha incorretos.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear(); // Apaga todos os dados da sessão do usuário logado
            TempData["Mensagem"] = "Você saiu da sua conta.";
            return Redirect("/?logout=1");
        }

        // --- MÉTODOS DE RECUPERAÇÃO DE SENHA ---
        [HttpGet]
        public IActionResult RecuperarSenha(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarSenha(string email, string telefone, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            // Validação dupla (2FA Acadêmico): Confere se e-mail e telefone batem no banco
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == email && c.Telefone == telefone);

            if (cliente != null)
            {
                TempData["IdRecuperacao"] = cliente.IdCliente;
                TempData["ReturnUrlRecuperacao"] = returnUrl;
                return RedirectToAction("RedefinirSenha");
            }

            ViewBag.Erro = "Os dados informados não conferem com nenhum cadastro.";
            return View();
        }

        [HttpGet]
        public IActionResult RedefinirSenha()
        {
            if (TempData["IdRecuperacao"] == null) return RedirectToAction("Login");

            TempData.Keep("IdRecuperacao");
            TempData.Keep("ReturnUrlRecuperacao");
            return View();
        }

        [HttpPost]
        public IActionResult RedefinirSenha(string novaSenha)
        {
            if (TempData["IdRecuperacao"] is int idCliente)
            {
                string? returnUrl = TempData["ReturnUrlRecuperacao"] as string;
                var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == idCliente);

                if (cliente != null && !string.IsNullOrWhiteSpace(novaSenha))
                {
                    cliente.SenhaAcesso = novaSenha;
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Senha redefinida com sucesso! Faça seu login.";
                    return RedirectToAction("Login", new { returnUrl });
                }
            }
            return RedirectToAction("Login");
        }
    }
}
