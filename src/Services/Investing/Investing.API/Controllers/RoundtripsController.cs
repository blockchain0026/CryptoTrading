using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.Investing.Domain.Model.Roundtrips;
using CryptoTrading.Services.Investing.Infrastructure;

namespace Investing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundtripsController : ControllerBase
    {
        private readonly InvestingContext _context;
        private readonly IRoundtripRepository _roundtripRepository;

        public RoundtripsController(InvestingContext context, IRoundtripRepository roundtripRepository)
        {
            _context = context;
            this._roundtripRepository = roundtripRepository ?? throw new ArgumentNullException(nameof(roundtripRepository));
        }


        // GET: api/Traces
        [Route("GetAllRoundtrips")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoundtrips()
        {
            var roundtrips = await this._roundtripRepository.GetAll();

            return Ok(roundtrips);
        }

        // GET: api/Roundtrips/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoundtrip([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roundtrip = await _context.Roundtrips.FindAsync(id);

            if (roundtrip == null)
            {
                return NotFound();
            }

            return Ok(roundtrip);
        }

        // PUT: api/Roundtrips/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoundtrip([FromRoute] int id, [FromBody] Roundtrip roundtrip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roundtrip.Id)
            {
                return BadRequest();
            }

            _context.Entry(roundtrip).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoundtripExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Roundtrips
        [HttpPost]
        public async Task<IActionResult> PostRoundtrip([FromBody] Roundtrip roundtrip)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Roundtrips.Add(roundtrip);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoundtrip", new { id = roundtrip.Id }, roundtrip);
        }

        // DELETE: api/Roundtrips/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoundtrip([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roundtrip = await _context.Roundtrips.FindAsync(id);
            if (roundtrip == null)
            {
                return NotFound();
            }

            _context.Roundtrips.Remove(roundtrip);
            await _context.SaveChangesAsync();

            return Ok(roundtrip);
        }

        private bool RoundtripExists(int id)
        {
            return _context.Roundtrips.Any(e => e.Id == id);
        }
    }
}