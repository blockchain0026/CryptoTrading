using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Balances;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using System.Net;

namespace ExchangeAccess.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalancesController : ControllerBase
    {
        private readonly ExchangeAccessContext _context;
        private readonly IBalanceRepository _balanceRepository;
        public BalancesController(ExchangeAccessContext context, IBalanceRepository balanceRepository)
        {
            _context = context;
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }


        // GET: api/Balances/AllBalances
        [Route("AllBalances")]
        [HttpGet]
        public async Task<IActionResult> GetAllBalances()
        {
            return Ok(await _balanceRepository.GetAll());
        }

        // GET: api/Balances
        [Route("GetBalance")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBalance(string username)
        {
            try
            {
                var balance = await this._balanceRepository.GetByUsernameAsync(username);

                if (balance == null)
                {
                    return NotFound();
                }


                var result = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Asset>();

                foreach (var asset in balance.Assets)
                {
                    result.Add(new CryptoTrading.Services.ExchangeAccess.API.Application.Models.Asset
                    {
                        Total = asset.TotalBalance(),
                        Available = asset.Available,
                        Locked = asset.Locked,
                        Symbol = asset.Symbol
                    });
                }

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }

        // GET: api/Balances/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBalance([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var balance = await _context.Balances.FindAsync(id);

            if (balance == null)
            {
                return NotFound();
            }

            return Ok(balance);
        }

        // PUT: api/Balances/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalance([FromRoute] int id, [FromBody] Balance balance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != balance.Id)
            {
                return BadRequest();
            }

            _context.Entry(balance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BalanceExists(id))
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

        // POST: api/Balances
        [HttpPost]
        public async Task<IActionResult> PostBalance([FromBody] Balance balance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBalance", new { id = balance.Id }, balance);
        }

        // DELETE: api/Balances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var balance = await _context.Balances.FindAsync(id);
            if (balance == null)
            {
                return NotFound();
            }

            _context.Balances.Remove(balance);
            await _context.SaveChangesAsync();

            return Ok(balance);
        }

        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.Id == id);
        }
    }
}