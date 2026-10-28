using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("Promocao")]
public class Promocao
{
    [Key]
    [Column("idPromocao")]
    public int IdPromocao { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("percentualDesconto")]
    public decimal? PercentualDesconto { get; set; }

    [Column("dataInicio")]
    public DateTime? DataInicio { get; set; }

    [Column("dataFim")]
    public DateTime? DataFim { get; set; }
}
