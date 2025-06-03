using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace CriptoWallet.Api.Models
{
    public class Criptomoneda
    {
        [Key]
        [MaxLength(50)]
        public string CryptoCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(20)]
        public string TipoValor { get; set; } // "estable" o "volatil"

        // Relaciones
        public List<Transaccion> Transacciones { get; set; }
    }
}
