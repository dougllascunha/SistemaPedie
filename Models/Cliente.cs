using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace pimFinal.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }
    
    public string Nome { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string? Telefone { get; set; }
    
    [Column("senhaAcesso")]
    [Required(ErrorMessage = "A senha é obrigatória")]
    public string SenhaAcesso { get; set; } = string.Empty;
    
    [Column("perfil")]
    public string Perfil { get; set; } = "Cliente";
    
    [Column("dataCadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
