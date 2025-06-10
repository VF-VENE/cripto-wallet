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
        public async Task<ActionResult<IEnumerable<TransaccionDto>>> Get()
        {
            var transaccionesDto = await _context.Transacciones
                .Select(t => new TransaccionDto
                {
                    ClienteID = t.ClienteID,
                    CryptoCode = t.CryptoCode,
                    Accion = t.Accion,
                    CantidadCripto = t.CantidadCripto,
                    Fecha = t.Fecha,
                    ExchangeID = t.ExchangeID
                })
                .ToListAsync();

            return Ok(transaccionesDto);
        }

        [HttpGet("historial")]
        public async Task<ActionResult<IEnumerable<object>>> Get([FromQuery] int? clienteId)
        {
            var query = _context.Transacciones
                .Include(t => t.Cliente)
                .Include(t => t.Exchange)
                .AsQueryable();

            if (clienteId.HasValue)
                query = query.Where(t => t.ClienteID == clienteId);

            var transacciones = await query
                .OrderByDescending(t => t.Fecha)
                .Select(t => new {
                    id = t.TransaccionID,
                    action = t.Accion,
                    crypto_code = t.CryptoCode,
                    client_id = t.ClienteID,
                    crypto_amount = t.CantidadCripto,
                    money = t.Monto,
                    datetime = t.Fecha,
                    nombreCliente = t.Cliente.Nombre,          // opcional si querés mostrarlo
                    nombreExchange = t.Exchange.Nombre // opcional también
                })
                .ToListAsync();

            return Ok(transacciones);
        }




        [HttpPost]
        public async Task<ActionResult<Transaccion>> Post([FromBody] TransaccionDto transaccionDto)
        {
            try
            {
                if (transaccionDto.CantidadCripto <= 0)
                    return BadRequest("Cantidad inválida");

                if (string.IsNullOrEmpty(transaccionDto.CryptoCode))
                    return BadRequest("Falta cryptoCode");

                if (transaccionDto.ClienteID <= 0)
                    return BadRequest("Cliente inválido");

                if (transaccionDto.Accion != "purchase" && transaccionDto.Accion != "sale")
                    return BadRequest("Acción inválida");

                if (transaccionDto.Fecha == DateTime.MinValue)
                    return BadRequest("Fecha inválida");

                if (transaccionDto.ExchangeID == null || transaccionDto.ExchangeID <= 0)
                    return BadRequest("Exchange inválido");

                decimal precio = await ObtenerPrecioCripto(transaccionDto.ExchangeID.Value, transaccionDto.CryptoCode);

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
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar transacción: " + ex.Message);
                Console.WriteLine(ex.StackTrace);

                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }



        // Método privado para consultar precio externo
        private async Task<decimal> ObtenerPrecioCripto(int exchangeId, string cryptoCode)
        {
            try
            {
                var exchangeEntity = await _context.Exchanges.FindAsync(exchangeId);
                if (exchangeEntity == null)
                    return 0;

                string codigoAPI = exchangeEntity.CodigoAPI;

                using var httpClient = new HttpClient();
                string url = $"https://criptoya.com/api/{codigoAPI}/{cryptoCode}/ars";

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
