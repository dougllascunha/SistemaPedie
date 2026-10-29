using Microsoft.AspNetCore.Mvc;
using pimFinal.Data;
using pimFinal.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace pimFinal.Controllers;

public class PedidoController : Controller
{
    private readonly PedieDbContext _context;

    public PedidoController(PedieDbContext context)
    {
        _context = context;
    }

    public IActionResult ChecarPedido()
    {
        var idCliente = HttpContext.Session.GetInt32("IdCliente");
        if (idCliente == null)
        {
            TempData["Mensagem"] = "Por favor, faça login ou crie sua conta para finalizar a compra.";
            return RedirectToAction("Login", "Cliente", new { returnUrl = "/Carrinho" });
        }

        var carrinhoJson = HttpContext.Session.GetString("Carrinho");
        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson ?? "[]") ?? new List<int>();

        if (!carrinho.Any())
        {
            TempData["Erro"] = "Seu carrinho está vazio. Adicione doces antes de finalizar o pedido.";
            return RedirectToAction("Index", "Carrinho");
        }

        var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == idCliente.Value);
        if (cliente == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Cliente", new { returnUrl = "/Carrinho" });
        }

        var enderecoSalvo = _context.Enderecos.FirstOrDefault(e => e.IdCliente == idCliente.Value);
        ViewBag.Endereco = enderecoSalvo;

        decimal subtotal = 0;
        var grupos = carrinho.GroupBy(id => id);

        foreach (var grupo in grupos)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == grupo.Key);
            if (produto != null) subtotal += produto.Preco * grupo.Count();
        }

        ViewBag.Subtotal = subtotal;
        ViewBag.Frete = 10m;
        ViewBag.TotalFinal = subtotal + 10m;

        return View(cliente);
    }

    [HttpPost]
    public IActionResult FinalizarCompra(
        string? entrega,
        string? pagamento,
        string? cep,
        string? rua,
        string? numero,
        string? bairro,
        string? cidade,
        string? estado,
        string? complemento)
    {
        var idCliente = HttpContext.Session.GetInt32("IdCliente");
        if (idCliente == null) return RedirectToAction("Login", "Cliente", new { returnUrl = "/Carrinho" });

        var carrinhoJson = HttpContext.Session.GetString("Carrinho");
        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson ?? "[]") ?? new List<int>();
        if (!carrinho.Any()) return RedirectToAction("Index", "Carrinho");

        string tipoEntrega = string.IsNullOrWhiteSpace(entrega) ? "Entrega" : entrega;
        string formaPagamento = string.IsNullOrWhiteSpace(pagamento) ? "PIX" : pagamento;

        // Se for Entrega em domicílio, valida se todos os campos obrigatórios foram preenchidos
        if (tipoEntrega == "Entrega")
        {
            if (string.IsNullOrWhiteSpace(cep) ||
                string.IsNullOrWhiteSpace(rua) ||
                string.IsNullOrWhiteSpace(numero) ||
                string.IsNullOrWhiteSpace(bairro) ||
                string.IsNullOrWhiteSpace(cidade) ||
                string.IsNullOrWhiteSpace(estado))
            {
                TempData["Erro"] = "Por favor, preencha todos os campos obrigatórios do endereço de entrega (CEP, Rua, Número, Bairro, Cidade e Estado).";
                return RedirectToAction("ChecarPedido");
            }

            // 1. Salva ou atualiza o endereço do cliente
            var endereco = _context.Enderecos.FirstOrDefault(e => e.IdCliente == idCliente.Value);
            string ufFormatada = estado.Trim().Length > 2 ? estado.Trim().Substring(0, 2).ToUpper() : estado.Trim().ToUpper();

            if (endereco == null)
            {
                endereco = new Endereco
                {
                    IdCliente = idCliente.Value,
                    Cep = cep.Trim(),
                    Rua = rua.Trim(),
                    Numero = numero.Trim(),
                    Bairro = bairro.Trim(),
                    Cidade = cidade.Trim(),
                    Uf = ufFormatada,
                    Complemento = complemento?.Trim()
                };
                _context.Enderecos.Add(endereco);
            }
            else
            {
                endereco.Cep = cep.Trim();
                endereco.Rua = rua.Trim();
                endereco.Numero = numero.Trim();
                endereco.Bairro = bairro.Trim();
                endereco.Cidade = cidade.Trim();
                endereco.Uf = ufFormatada;
                endereco.Complemento = complemento?.Trim();
            }
            _context.SaveChanges();
        }

        HttpContext.Session.SetString("TipoEntrega", tipoEntrega);
        HttpContext.Session.SetString("FormaPagamento", formaPagamento);

        // 2. Redireciona para a tela de Pagamento
        return RedirectToAction("Pagamento");
    }

    [HttpGet]
    public IActionResult Pagamento()
    {
        var idCliente = HttpContext.Session.GetInt32("IdCliente");
        if (idCliente == null) return RedirectToAction("Login", "Cliente", new { returnUrl = "/Carrinho" });

        var carrinhoJson = HttpContext.Session.GetString("Carrinho");
        if (string.IsNullOrEmpty(carrinhoJson)) return RedirectToAction("Index", "Carrinho");

        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson) ?? new List<int>();
        if (!carrinho.Any()) return RedirectToAction("Index", "Carrinho");

        decimal subtotal = 0;
        var grupos = carrinho.GroupBy(id => id);
        foreach (var grupo in grupos)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == grupo.Key);
            if (produto != null) subtotal += produto.Preco * grupo.Count();
        }

        string tipoEntrega = HttpContext.Session.GetString("TipoEntrega") ?? "Entrega";
        decimal frete = tipoEntrega == "Retirada" ? 0m : 10m;

        ViewBag.Subtotal = subtotal;
        ViewBag.Frete = frete;
        ViewBag.TotalFinal = subtotal + frete;
        ViewBag.FormaPagamento = HttpContext.Session.GetString("FormaPagamento") ?? "PIX";
        return View();
    }

    [HttpPost]
    public IActionResult ConfirmarPagamento()
    {
        var idCliente = HttpContext.Session.GetInt32("IdCliente");
        if (idCliente == null) return RedirectToAction("Login", "Cliente", new { returnUrl = "/Carrinho" });

        var carrinhoJson = HttpContext.Session.GetString("Carrinho");
        var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson ?? "[]") ?? new List<int>();
        if (!carrinho.Any()) return RedirectToAction("Index", "Carrinho");

        // Garante que a situação 2 ("Preparando") exista no banco para evitar erro de chave estrangeira
        var situacaoPreparando = _context.Situacoes.FirstOrDefault(s => s.IdSituacao == 2)
                                 ?? _context.Situacoes.FirstOrDefault();

        int idSituacaoInicial = situacaoPreparando?.IdSituacao ?? 2;

        // Cria o Pedido final no banco com status 2 (Preparando/Pago)
        var pedido = new Pedido
        {
            IdCliente = idCliente.Value,
            IdSituacao = idSituacaoInicial,
            DataPedido = DateTime.Now,
            valorTotal = 0
        };

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();

        decimal totalPedido = 0;
        var grupos = carrinho.GroupBy(id => id);

        foreach (var grupo in grupos)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == grupo.Key);
            if (produto == null) continue;

            var itemPedido = new ItemPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = grupo.Count(),
                PrecoUnitario = produto.Preco
            };
            _context.ItensPedido.Add(itemPedido);
            totalPedido += produto.Preco * grupo.Count();
        }

        string tipoEntrega = HttpContext.Session.GetString("TipoEntrega") ?? "Entrega";
        decimal frete = tipoEntrega == "Retirada" ? 0m : 10m;

        pedido.valorTotal = totalPedido + frete;
        _context.SaveChanges();

        HttpContext.Session.Remove("Carrinho");
        HttpContext.Session.Remove("TipoEntrega");
        HttpContext.Session.Remove("FormaPagamento");

        return RedirectToAction("Sucesso");
    }

    public IActionResult Sucesso()
    {
        return View();
    }

    public IActionResult MeusPedidos()
    {
        var idCliente = HttpContext.Session.GetInt32("IdCliente");
        if (idCliente == null) return RedirectToAction("Login", "Cliente", new { returnUrl = "/Pedido/MeusPedidos" });

        var meusPedidos = _context.Pedidos
            .Include(p => p.Situacao)
            .Where(p => p.IdCliente == idCliente.Value)
            .OrderByDescending(p => p.DataPedido)
            .ToList();
        return View(meusPedidos);
    }

    // --- FUNÇÕES DO ADMIN ---
    public IActionResult GerenciarPedidos()
    {
        if (HttpContext.Session.GetString("Perfil") != "Admin")
            return RedirectToAction("Index", "Home");

        var todosPedidos = _context.Pedidos
            .Include(p => p.Situacao)
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.DataPedido)
            .ToList();

        var ordemFluxo = new Dictionary<int, int>
        {
            { 1, 1 }, // Aguardando Pagamento
            { 2, 2 }, // Preparando
            { 5, 3 }, // Saiu para Entrega
            { 3, 4 }, // Pronto para Retirada na Loja
            { 4, 5 }, // Entregue
            { 6, 6 }  // Cancelado
        };

        ViewBag.Situacoes = _context.Situacoes
            .ToList()
            .OrderBy(s => ordemFluxo.TryGetValue(s.IdSituacao, out int ordem) ? ordem : 99)
            .ToList();
        return View(todosPedidos);
    }

    [HttpPost]
    public IActionResult AtualizarStatus(int idPedido, int idSituacao)
    {
        if (HttpContext.Session.GetString("Perfil") != "Admin")
            return RedirectToAction("Index", "Home");

        var pedido = _context.Pedidos.FirstOrDefault(p => p.IdPedido == idPedido);
        if (pedido != null)
        {
            pedido.IdSituacao = idSituacao;
            _context.SaveChanges();
            TempData["Mensagem"] = $"Status do pedido #{idPedido} atualizado com sucesso!";
        }
        return RedirectToAction("GerenciarPedidos");
    }
}
