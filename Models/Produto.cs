using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("Produto")]
public class Produto
{
    [Key]
    public int IdProduto { get; set; }

    public string Nome { get; set; } = string.Empty;// STRING.EMPTY NÃO FICA NULL RECEBE "" 

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

    public bool Disponivel { get; set; }

    public string? ImagemUrl { get; set; }

    public int IdCategoria { get; set; }
}