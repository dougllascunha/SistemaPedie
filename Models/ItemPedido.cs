using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("ItemPedido")]
public class ItemPedido
{
    [Key]
    public int IdItemPedido { get; set; }

    public int IdPedido { get; set; }

    public int IdProduto { get; set; }

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    [ForeignKey("IdPedido")]
    public Pedido Pedido { get; set; } = null!;

    [ForeignKey("IdProduto")]
    public Produto Produto { get; set; } = null!;
}