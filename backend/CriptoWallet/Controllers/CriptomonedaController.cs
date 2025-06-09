using CriptoWallet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriptomonedaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CriptomonedaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCriptomonedas()
        {
            var criptos = await _context.Criptomonedas.ToListAsync();
            return Ok(criptos);
        }
    }
}
