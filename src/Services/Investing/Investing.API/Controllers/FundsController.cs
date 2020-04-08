using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.Investing.Domain.Model.Funds;
using CryptoTrading.Services.Investing.Infrastructure;
using System.Net;
using CryptoTrading.Services.Investing.API.Application.Commands;
using MediatR;

namespace Investing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundsController : ControllerBase
    {
        private readonly InvestingContext _context;
        private readonly IFundRepository _fundRepository;
        private readonly IMediator _mediator;

        public FundsController(InvestingContext context, IFundRepository fundRepository, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fundRepository = fundRepository ?? throw new ArgumentNullException(nameof(fundRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }




        // GET: api/Funds
        [HttpGet]
        public async Task<IActionResult> GetFunds()
        {
            return Ok(await this._fundRepository.GetAll());
        }

        // GET: api/Funds/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFund([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fund = await _context.Funds.FindAsync(id);

            if (fund == null)
            {
                return NotFound();
            }

            return Ok(fund);
        }

        /// <summary>
        /// 分配實際資產給新投資
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Funds/AllocateInvestmentFund
        ///     {
        ///        "FundId": "abc123asd9",
        ///        "InvestmentId": "35asd65dfg",
        ///        "Quantity":100,
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// 
        /// </param>
        ///
        /// <param name="requestId">
        /// </param> 
        /// 
        /// 
        [Route("AllocateInvestmentFund")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateInvestmentFund([FromBody] AllocateInvestmentFundCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var request = new IdentifiedCommand<AllocateInvestmentFundCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(request);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }


        // PUT: api/Funds/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFund([FromRoute] int id, [FromBody] Fund fund)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fund.Id)
            {
                return BadRequest();
            }

            _context.Entry(fund).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FundExists(id))
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

        // POST: api/Funds
        [HttpPost]
        public async Task<IActionResult> PostFund([FromBody] Fund fund)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Funds.Add(fund);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFund", new { id = fund.Id }, fund);
        }

        // DELETE: api/Funds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFund([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fund = await _context.Funds.FindAsync(id);
            if (fund == null)
            {
                return NotFound();
            }

            _context.Funds.Remove(fund);
            await _context.SaveChangesAsync();

            return Ok(fund);
        }

        private bool FundExists(int id)
        {
            return _context.Funds.Any(e => e.Id == id);
        }
    }
}