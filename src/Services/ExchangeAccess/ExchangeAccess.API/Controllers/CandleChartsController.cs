using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.CandleCharts;
using CryptoTrading.Services.ExchangeAccess.Infrastructure;
using System.Net;
using CryptoTrading.Services.ExchangeAccess.API.Application.Commands;
using CryptoTrading.Services.ExchangeAccess.Domain.Model.Markets;

namespace ExchangeAccess.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandleChartsController : ControllerBase
    {
        private readonly ExchangeAccessContext _context;
        private readonly ICandleChartRepository _candleChartRepository;
        private readonly IExchangeAccessService _exchangeAccessService;

        public CandleChartsController(ExchangeAccessContext context, ICandleChartRepository candleChartRepository, IExchangeAccessService exchangeAccessService)
        {
            _context = context;
            _candleChartRepository = candleChartRepository ?? throw new ArgumentNullException(nameof(candleChartRepository));
            this._exchangeAccessService = exchangeAccessService ?? throw new ArgumentNullException(nameof(exchangeAccessService));
        }


        // GET: api/CandleCharts/AllCandleCharts
        [Route("AllCandleCharts")]
        [HttpGet]
        public async Task<IActionResult> GetAllCandleCharts()
        {
            return Ok(await _candleChartRepository.GetAll());
        }

        // GET: api/CandleCharts/5
        [Route("GetCandleChart")]
        [HttpGet]
        public async Task<IActionResult> GetByCandleChartId(string id)
        {
            var candleChart = await _candleChartRepository.GetByCandleChartIdAsync(id);

            if (candleChart == null)
            {
                return NotFound();
            }

            var result = new List<CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle>();

            foreach(var candle in candleChart.Candles)
            {
                result.Add(new CryptoTrading.Services.ExchangeAccess.API.Application.Models.Candle
                {
                    Timestamp=candle.Timestamp,
                    Open=candle.Open,
                    Close=candle.Close,
                    High=candle.High,
                    Low=candle.Low,
                    Volume=candle.Volume
                });
            }

            return Ok(result);
        }

        // POST: api/CandleCharts/
        [Route("UpdateCandlesFromExchange")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCandlesFromExchange([FromBody] UpdateCandlesFromExchangeCommand command)
        {
            /* if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             */

            try
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                var from = dtDateTime.AddSeconds(command.From);
                var to = dtDateTime.AddSeconds(command.To);

                var candles = await this._exchangeAccessService.GetCandlesData(
                    command.ExchangeId,
                    command.BaseCurrency,
                    command.QuoteCurrency,
                    CandlePeriod.FromName(command.CandlePeriod),
                    from,
                    to);

                var chart = await this._candleChartRepository
                    .GetByCurrencyPairAsync(command.BaseCurrency, command.QuoteCurrency, command.ExchangeId, CandlePeriod.FromName(command.CandlePeriod));

                if (chart == null)
                {
                    throw new KeyNotFoundException();
                }

                foreach (var candle in candles)
                {
                    chart.UpdateCandle(candle.Timestamp, candle.High, candle.Low, candle.Open, candle.Close, candle.Volume);
                }

                await _context.SaveEntitiesAsync();

                return Ok(chart.CandleChartId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        // POST: api/CandleCharts
        [HttpPost]
        public async Task<IActionResult> PostCandleChart([FromBody] CandleChart candleChart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CandleCharts.Add(candleChart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCandleChart", new { id = candleChart.Id }, candleChart);
        }

        // DELETE: api/CandleCharts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandleChart([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candleChart = await _context.CandleCharts.FindAsync(id);
            if (candleChart == null)
            {
                return NotFound();
            }

            _context.CandleCharts.Remove(candleChart);
            await _context.SaveChangesAsync();

            return Ok(candleChart);
        }

        private bool CandleChartExists(int id)
        {
            return _context.CandleCharts.Any(e => e.Id == id);
        }
    }
}