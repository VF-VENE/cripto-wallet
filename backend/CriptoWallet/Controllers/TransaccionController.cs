using CriptoWallet.Api.Models;
using CriptoWallet.Data;
using System.Text.Json;
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
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial([FromQuery] int? clienteId)
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
                    nombreCliente = t.Cliente.Nombre,
                    nombreExchange = t.Exchange.Nombre
                })
                .ToListAsync();

            return Ok(transacciones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var t = await _context.Transacciones
                .Include(t => t.Cliente)
                .Include(t => t.Exchange)
                .FirstOrDefaultAsync(x => x.TransaccionID == id);

            if (t == null) return NotFound("Transacción no encontrada");

            return Ok(new
            {
                id = t.TransaccionID,
                action = t.Accion,
                crypto_code = t.CryptoCode,
                client_id = t.ClienteID,
                crypto_amount = t.CantidadCripto,
                money = t.Monto,
                datetime = t.Fecha,
                nombreCliente = t.Cliente.Nombre,
                nombreExchange = t.Exchange.Nombre
            });
        }

        [HttpPost]
        public async Task<ActionResult<Transaccion>> Post([FromBody] TransaccionDto transaccionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (transaccionDto.CantidadCripto <= 0) return BadRequest("La cantidad debe ser mayor a 0.");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteID == transaccionDto.ClienteID);
            if (!clienteExiste) return BadRequest("El cliente no existe.");

            var exchangeExiste = await _context.Exchanges.AnyAsync(e => e.ExchangeID == transaccionDto.ExchangeID);
            if (!exchangeExiste) return BadRequest("El exchange no existe.");

            if (transaccionDto.Accion == "sale")
            {
                var compras = await _context.Transacciones
                    .Where(t => t.ClienteID == transaccionDto.ClienteID && t.CryptoCode == transaccionDto.CryptoCode && t.Accion == "purchase")
                    .SumAsync(t => t.CantidadCripto);

                var ventas = await _context.Transacciones
                    .Where(t => t.ClienteID == transaccionDto.ClienteID && t.CryptoCode == transaccionDto.CryptoCode && t.Accion == "sale")
                    .SumAsync(t => t.CantidadCripto);

                decimal saldoDisponible = compras - ventas;

                if (transaccionDto.CantidadCripto > saldoDisponible)
                    return BadRequest("Saldo insuficiente para realizar la venta.");
            }

            decimal precio = await ObtenerPrecioCripto(transaccionDto.ExchangeID.Value, transaccionDto.CryptoCode);
            if (precio <= 0) return BadRequest("No se pudo obtener el precio del exchange.");

            var transaccion = new Transaccion
            {
                ClienteID = transaccionDto.ClienteID,
                CryptoCode = transaccionDto.CryptoCode.ToLower(),
                Accion = transaccionDto.Accion,
                CantidadCripto = transaccionDto.CantidadCripto,
                Fecha = transaccionDto.Fecha,
                Monto = precio * transaccionDto.CantidadCripto,
                ExchangeID = transaccionDto.ExchangeID
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = transaccion.TransaccionID }, transaccion);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonElement updates)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound("Transacción no encontrada");

            if (updates.TryGetProperty("datetime", out var dateProp))
            {
                transaccion.Fecha = dateProp.GetDateTime();
            }

            if (updates.TryGetProperty("crypto_amount", out var amountProp))
            {
                decimal nuevaCantidad = amountProp.GetDecimal();
                if (nuevaCantidad <= 0) return BadRequest("La cantidad debe ser mayor a 0.");

                decimal precioActual = await ObtenerPrecioCripto(transaccion.ExchangeID.Value, transaccion.CryptoCode);
                if (precioActual > 0)
                {
                    transaccion.Monto = precioActual * nuevaCantidad;
                }
                transaccion.CantidadCripto = nuevaCantidad;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();

            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<decimal> ObtenerPrecioCripto(int exchangeId, string cryptoCode)
        {
            try
            {
                var exchangeEntity = await _context.Exchanges.FindAsync(exchangeId);
                if (exchangeEntity == null) return 0;

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