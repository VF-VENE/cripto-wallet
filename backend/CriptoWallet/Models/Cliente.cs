using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Transactions;

namespace CriptoWallet.Api.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relaciones
        public List<Transaccion> Transacciones { get; set; }
    }
}
