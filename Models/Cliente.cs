using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pimFinal.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }
     
    public string Nome { get; set; } = string.Empty;
     
    public string Email { get; set; } = string.Empty;
     
    public string Telefone { get; set; } = string.Empty;
     
    public string Endereco { get; set; } = string.Empty;
}