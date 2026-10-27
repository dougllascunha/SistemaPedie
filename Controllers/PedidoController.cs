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
                return RedirectToAction("Login", "Cliente", new { returnUrl = "/Pedido/ChecarPedido"});
            }

            var cliente = _context.Clientes
                .FirstOrDefault(c => c.IdCliente == idCliente);
                var carrinhoJson = HttpContext.Session.GetString("Carrinho");

                var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson ?? "[]")
                    ?? new List<int>();

                    decimal subtotal = 0;

                var grupos = carrinho.GroupBy(id => id);

                    foreach (var grupo in grupos)
                    {
                        var produto = _context.Produtos
                            .FirstOrDefault(p => p.IdProduto == grupo.Key);

                        if (produto != null)
                            {
                                subtotal += produto.Preco * grupo.Count();
                            }
                    }

                decimal frete = 10m;

                ViewBag.Subtotal = subtotal;
                ViewBag.Frete = frete;
                ViewBag.TotalFinal = subtotal + frete;

            return View(cliente);
        }

        [HttpPost]
        public IActionResult FinalizarCompra
        (
            string cep,
            string rua,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string complemento
        )

        {
            var idCliente = HttpContext.Session.GetInt32("IdCliente");

            if (idCliente == null)
            {
                return RedirectToAction("Login", "Cliente", new { returnUrl = "/Pedido/ChecarPedido" });
            }
            var endereco = _context.Enderecos
                .FirstOrDefault(e => e.IdCliente == idCliente.Value);

            if (endereco == null)
            {
                endereco = new Endereco
                {
                    IdCliente = idCliente.Value,
                    Cep = cep,
                    Rua = rua,
                    Numero = numero,
                    Bairro = bairro,
                    Cidade = cidade,
                    Uf = estado,
                    Complemento = complemento
                };

                _context.Enderecos.Add(endereco);
            }
            else
            {
                endereco.Cep = cep;
                endereco.Rua = rua;
                endereco.Numero = numero;
                endereco.Bairro = bairro;
                endereco.Cidade = cidade;
                endereco.Uf = estado;
                endereco.Complemento = complemento;
            }
            

            var carrinhoJson = HttpContext.Session.GetString("Carrinho");

            if (string.IsNullOrEmpty(carrinhoJson))
                {
                    return RedirectToAction("Index", "Carrinho");
                }

            var carrinho = JsonSerializer.Deserialize<List<int>>(carrinhoJson)
                        ?? new List<int>();

            var pedido = new Pedido
                {
                    IdCliente = idCliente.Value,
                    IdSituacao = 1,
                    DataPedido = DateTime.Now,
                    valorTotal = 0
                };

            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            decimal totalPedido = 0;

            var grupos = carrinho.GroupBy(id => id);

            foreach (var grupo in grupos)
            {
                var produto = _context.Produtos
                    .FirstOrDefault(p => p.IdProduto == grupo.Key);

                if (produto == null)
                    continue;

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

            pedido.valorTotal = totalPedido;

            _context.SaveChanges();

            HttpContext.Session.Remove("Carrinho");
            return RedirectToAction("Sucesso");
        }
        public IActionResult Sucesso()
        public IActionResult MeusPedidos()
        {
            var idCliente = HttpContext.Session.GetInt32("IdCliente");

            if (idCliente == null)
            {
                return RedirectToAction("Login", "Cliente");
            }

            // Busca os pedidos do cliente, incluindo a descrição da Situação
            var meusPedidos = _context.Pedidos
                .Include(p => p.Situacao)
                .Where(p => p.IdCliente == idCliente.Value)
                .OrderByDescending(p => p.DataPedido) 
                .ToList();

            return View(meusPedidos);
        }
        {
            return View();
        }

}
