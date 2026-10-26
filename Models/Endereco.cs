using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pimFinal.Models
{
    [Table("Endereco")]
    public class Endereco
    {
        [Key]
        public int IdEndereco { get; set; }

        public int IdCliente { get; set; }

        public string? Cep { get; set; }

        public string? Rua { get; set; }

        public string? Numero { get; set; }

        public string? Bairro { get; set; }

        public string? Cidade { get; set; }

        public string? Uf { get; set; }

        public string? Complemento { get; set; }
    }
}