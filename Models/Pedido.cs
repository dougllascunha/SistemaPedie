using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("Pedido")]
public class Pedido
{
    [Key]
    public int IdPedido   { get; set; }
    public int IdCliente  { get; set; }
    public int IdSituacao { get; set; }

    public DateTime DataPedido { get; set; } = DateTime.Now;

    [Column("valorTotal")]
    public decimal valorTotal { get; set; }


    [ForeignKey("IdCliente")]
    public Cliente Cliente { get; set; } = null!;
}