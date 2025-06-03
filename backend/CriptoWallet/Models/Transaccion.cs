using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CriptoWallet.Api.Models
{
    public class Transaccion
    {
        public int TransaccionID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        [MaxLength(50)]
        public string CryptoCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string Accion { get; set; } // "purchase" o "sale"

        [Column(TypeName = "decimal(18,8)")]
        public decimal CantidadCripto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        public int? ExchangeID { get; set; }

        // Relaciones
        public Cliente Cliente { get; set; }
        public Criptomoneda Criptomoneda { get; set; }
        public Exchange Exchange { get; set; }
    }
}
