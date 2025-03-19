using System.ComponentModel.DataAnnotations;

namespace SYSMANU.Models
{
    public class Cliente
    {
        [Required]
        public int? COD_CLIENTE { get; set; }

        [Required]
        public string? NOME { get; set; }

       [Required]
        public string? TELEFONE { get; set; }

        [Required]
        public string? ENDERECO { get; set; }

        public string? CIDADE {get; set;}


    }
}
