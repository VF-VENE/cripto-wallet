namespace CriptoWallet.Models
{
    public class TransaccionDto
    {
        public int ClienteID { get; set; }
        public string CryptoCode { get; set; }
        public string Accion { get; set; }  // "purchase" o "sale"
        public decimal CantidadCripto { get; set; }
        public DateTime Fecha { get; set; }
        public int? ExchangeID { get; set; }
    }
}
