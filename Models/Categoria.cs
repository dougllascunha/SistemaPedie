using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("Categoria")]
public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = string.Empty;
}