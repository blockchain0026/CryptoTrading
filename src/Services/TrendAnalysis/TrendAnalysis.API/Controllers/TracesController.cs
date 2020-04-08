using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.TrendAnalysis.Domain.Model.Traces;
using CryptoTrading.Services.TrendAnalysis.Infrastructure;
using System.Net;
using CryptoTrading.Services.TrendAnalysis.API.Application.Commands;
using MediatR;

namespace TrendAnalysis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracesController : ControllerBase
    {
        private readonly TrendAnalysisContext _context;
        private readonly ITraceRepository _traceRepository;
        private readonly IMediator _mediator;

        public TracesController(TrendAnalysisContext context, ITraceRepository traceRepository, IMediator mediator)
        {
            _context = context;
            _traceRepository = traceRepository ?? throw new ArgumentNullException(nameof(traceRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // GET: api/Traces
        [Route("GetAllTraces")]
        [HttpGet]
        public async Task<IActionResult> GetAllTraces()
        {
            var traces = await this._traceRepository.GetAll();

            return Ok(traces);
        }

        // GET: api/Traces/5
        [HttpGet("{traceId}")]
        public async Task<IActionResult> GetTrace([FromRoute] string traceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trace = await _traceRepository.GetByTraceIdAsync(traceId);

            if (trace == null)
            {
                return NotFound();
            }

            return Ok(trace);
        }



        /*[Route("CreateTrace")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateTrace([FromBody] CreateTraceCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var requestShipOrder = new IdentifiedCommand<CreateTraceCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(requestShipOrder);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }

        }*/



        /// <summary>
        /// 新增交易策略
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Traces/AddStrategiesToTrace
        ///     {
        ///        "TraceId": "as98sd76df54",
        ///        "Strategies": ["RSI MACD"]
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
        [Route("AddStrategiesToTrace")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddStrategiesToTrace([FromBody] AddStrategiesCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var request = new IdentifiedCommand<AddStrategiesCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(request);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }

        }



        // PUT: api/Traces/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrace([FromRoute] int id, [FromBody] Trace trace)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trace.Id)
            {
                return BadRequest();
            }

            _context.Entry(trace).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TraceExists(id))
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

        // POST: api/Traces
        [HttpPost]
        public async Task<IActionResult> PostTrace([FromBody] Trace trace)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Traces.Add(trace);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrace", new { id = trace.Id }, trace);
        }

        // DELETE: api/Traces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrace([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trace = await _context.Traces.FindAsync(id);
            if (trace == null)
            {
                return NotFound();
            }

            _context.Traces.Remove(trace);
            await _context.SaveChangesAsync();

            return Ok(trace);
        }

        private bool TraceExists(int id)
        {
            return _context.Traces.Any(e => e.Id == id);
        }
    }
}