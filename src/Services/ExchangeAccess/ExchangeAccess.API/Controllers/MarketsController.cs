using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using CryptoTrading.Services.ExchangeAccess.API.Application.Commands;
using System.Net;

namespace ExchangeAccess.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketsController : ControllerBase
    {
        private readonly ExchangeAccessContext _context;
        private readonly IMarketRepository _marketRepository;
        private readonly IExchangeAccessService _exchangeAccessService;

        public MarketsController(ExchangeAccessContext context, IMarketRepository marketRepository, IExchangeAccessService exchangeAccessService)
        {
            _context = context;
            this._marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this._exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }

        // GET: api/Markets/AllMarket
        [Route("AllMarket")]
        [HttpGet]
        public async Task<IActionResult> GetAllMarkets()
        {
            var markets = await _marketRepository.GetAll();

            var orderBooks = new List<OrderBook>();

            foreach (var market in markets)
            {
                orderBooks.Add(market.GetOrderBook());
            }

            return Ok(orderBooks);
        }

        // GET: api/Markets/GetMarket?marketId=abc123
        [Route("GetMarket")]
        [HttpGet]
        public async Task<IActionResult> GetMarket(string marketId)
        {
            var market = await _marketRepository.GetByMarketIdAsync(marketId);

            if (market == null)
            {
                return NotFound();
            }

            return Ok(market);
        }

        // PUT: api/Markets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> KeepUpdateMarket([FromRoute] int id, [FromBody] Market market)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != market.Id)
            {
                return BadRequest();
            }

            _context.Entry(market).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarketExists(id))
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

        // POST: api/Markets
        [Route("CreateMarket")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateMarket([FromBody] CreateMarketCommand command)
        {

            try
            {
                var market = await Market.FromAccessService(this._exchangeAccessService, command.ExchangeId, command.BaseCurrency.ToUpper(), command.QuoteCurrency.ToUpper());
                if (market != null)
                {
                    this._marketRepository.Add(market);

                    await _context.SaveEntitiesAsync();
                }

                return Ok(market.MarketId);
                //return CreatedAtAction("GetMarket", new { marketId = market.MarketId }, market);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        private bool MarketExists(int id)
        {
            return _context.Markets.Any(e => e.Id == id);
        }
    }
}