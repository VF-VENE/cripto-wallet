using CriptoWallet.Api.Models;
using CriptoWallet.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CriptoWallet.Models;


namespace CriptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransaccionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> Get()
        {
            return await _context.Transacciones.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Transaccion>> Post([FromBody] TransaccionDto transaccionDto)
        {
            if (transaccionDto.CantidadCripto <= 0)
                return BadRequest("La cantidad debe ser mayor a 0");

            if (string.IsNullOrEmpty(transaccionDto.CryptoCode))
                return BadRequest("Debe especificar el código de la criptomoneda");

            if (transaccionDto.ClienteID <= 0)
                return BadRequest("Debe especificar un cliente válido");

            if (transaccionDto.Accion != "purchase" && transaccionDto.Accion != "sale")
                return BadRequest("La acción debe ser 'purchase' o 'sale'");

            if (transaccionDto.Fecha == DateTime.MinValue)
                return BadRequest("Debe especificar la fecha y hora de la transacción");
            if (transaccionDto.ExchangeID == null || transaccionDto.ExchangeID <= 0)
                return BadRequest("Debe especificar un Exchange válido");

            var exchangeEntity = await _context.Exchanges.FindAsync(transaccionDto.ExchangeID);
            if (exchangeEntity == null)
                return BadRequest("El Exchange especificado no existe");
            string exchangeCodigo = exchangeEntity.CodigoAPI;

            decimal precio = await ObtenerPrecioCripto(exchangeCodigo, transaccionDto.CryptoCode);
            if (precio <= 0)
                return BadRequest("No se pudo obtener el precio de la criptomoneda");

            var transaccion = new Transaccion
            {
                ClienteID = transaccionDto.ClienteID,
                CryptoCode = transaccionDto.CryptoCode,
                Accion = transaccionDto.Accion,
                CantidadCripto = transaccionDto.CantidadCripto,
                Fecha = transaccionDto.Fecha,
                Monto = precio * transaccionDto.CantidadCripto,
                ExchangeID = transaccionDto.ExchangeID
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = transaccion.TransaccionID }, transaccion);
        }


        // Método privado para consultar precio externo
        private async Task<decimal> ObtenerPrecioCripto(string exchange, string cryptoCode)
        {
            try
            {
                using var httpClient = new HttpClient();
                // Pone acá la URL correcta de la API real
                string url = $"https://criptoya.com/api/{exchange}/{cryptoCode}/ars";

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return 0;

                var json = await response.Content.ReadAsStringAsync();

                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ask", out var priceElement))
                {
                    return priceElement.GetDecimal();
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
