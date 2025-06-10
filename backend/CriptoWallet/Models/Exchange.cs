using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace CriptoWallet.Api.Models
{
    public class Exchange
    {
        public int ExchangeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(255)]
        public string URL { get; set; }

        [MaxLength(50)]
        public string CodigoAPI { get; set; }

        // Relaciones
        //public List<Transaccion> Transacciones { get; set; }
    }
}
