using CriptoWallet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CriptoWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExchangeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchanges()
        {
            var exchanges = await _context.Exchanges.ToListAsync();
            return Ok(exchanges);
        }
    }
}
