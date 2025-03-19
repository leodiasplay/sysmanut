using System.ComponentModel.DataAnnotations;

namespace SYSMANU.Models
{
    public class Usuario
    {
        [Required]
        public string? COD_USUARIO { get; set; }

        [Required]
        public int MATRICULA { get; set; }

        [Required]
        public string? SENHA { get; set; }

        [Required, EmailAddress]
        public string? EMAIL { get; set; }

        [Required]
        public int NIVEL { get; set; }

        [Required]
        public string? CARGO { get; set; }

        [Required]
        public string? NOME { get; set; }
    }
}
