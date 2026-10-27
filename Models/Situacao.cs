using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models;

[Table("Situacao")]
public class Situacao
{
    [Key]
    [Column("idSituacao")]
    public int IdSituacao { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;
}
