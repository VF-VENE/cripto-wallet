using CriptoWallet.Api.Models;
using CriptoWallet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CriptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existeEmail = await _context.Clientes.AnyAsync(c => c.Email == cliente.Email);
            if (existeEmail) return BadRequest("El email ya se encuentra registrado.");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteID }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, [FromBody] Cliente cliente)
        {
            if (id != cliente.ClienteID) return BadRequest("El ID no coincide");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente == null) return NotFound();

            var emailDuplicado = await _context.Clientes
                .AnyAsync(c => c.Email == cliente.Email && c.ClienteID != id);

            if (emailDuplicado) return BadRequest("El email ya está siendo utilizado por otro cliente.");

            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Email = cliente.Email;
            clienteExistente.Telefono = cliente.Telefono;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            var tieneTransacciones = await _context.Transacciones.AnyAsync(t => t.ClienteID == id);
            if (tieneTransacciones)
                return BadRequest("No se puede eliminar un cliente que tiene transacciones registradas.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.ClienteID == id);
        }
    }
}