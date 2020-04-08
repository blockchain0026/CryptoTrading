using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.Services.Investing.Domain.Model.Investments;
using CryptoTrading.Services.Investing.Infrastructure;
using System.Net;
using CryptoTrading.Services.Investing.API.Application.Commands;
using MediatR;

namespace Investing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentsController : ControllerBase
    {
        private readonly InvestingContext _context;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IMediator _mediator;

        public InvestmentsController(InvestingContext context, IInvestmentRepository investmentRepository, IMediator mediator)
        {
            _context = context;
            _investmentRepository = investmentRepository ?? throw new ArgumentNullException(nameof(investmentRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        // GET: api/Traces
        [Route("GetAllInvestments")]
        [HttpGet]
        public async Task<IActionResult> GetAllInvestments()
        {
            var investments = await this._investmentRepository.GetAll();

            return Ok(investments);
        }

        // GET: api/Investments/5
        [HttpGet("{investemntId}")]
        public async Task<IActionResult> GetInvestment([FromRoute] string investmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var investment = await _investmentRepository.GetByInvestmentId(investmentId);

            if (investment == null)
            {
                return NotFound();
            }

            return Ok(investment);
        }


        /// <summary>
        /// 建立新投資
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Investments/CreateInvestment
        ///     {
        ///        "InvestmentType": "backtesting",
        ///        "ExchangeId": 0,
        ///        "BaseCurrency": "BTC",
        ///        "QuoteCurrency": "USDT",
        ///        "Username":"test",
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
        [Route("CreateInvestment")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateInvestment([FromBody] CreateInvestmentCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var request = new IdentifiedCommand<CreateInvestmentCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(request);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }


        /// <summary>
        /// 設定回測時間段
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Investments/SetBacktestingPeriod
        ///     {
        ///        "InvestmentId": "a12b34c56",
        ///        "From": 1544227200,
        ///        "To": 1544313600
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// 
        /// </param>     
        /// 
        [Route("SetBacktestingPeriod")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetBacktestingPeriod([FromBody] SetPeriodForBacktestingCommand command)
        {
            bool commandResult = false;
            try
            {
                commandResult = await _mediator.Send(command);

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }



        /// <summary>
        /// 設定模擬資金
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Investments/SetSimulateFund
        ///     {
        ///        "InvestmentId": "a12b34c56",
        ///        "Quantity": 100
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// 
        /// </param>     
        /// 
        [Route("SetSimulateFund")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetSimulateFund([FromBody] SetSimulateFundCommand command)
        {
            bool commandResult = false;
            try
            {

                commandResult = await _mediator.Send(command);


                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }


        /// <summary>
        /// 標記投資為數值設定完成
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Investments/SettleInvestment
        ///     {
        ///        "InvestmentId": "a12b34c56"
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// </param>     
        /// 
        /// <param name="requestId">
        /// </param> 
        [Route("SettleInvestment")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SettleInvestment([FromBody] SettleInvestmentCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var request = new IdentifiedCommand<SettleInvestmentCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(request);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }



        /// <summary>
        /// 開始投資
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Investments/StartInvestment
        ///     {
        ///        "InvestmentId": "a12b34c56"
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// </param>     
        /// 
        /// <param name="requestId">
        /// </param> 
        [Route("StartInvestment")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartInvestment([FromBody] StartInvestmentCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;
            try
            {
                if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                {
                    var request = new IdentifiedCommand<StartInvestmentCommand, bool>(command, guid);
                    commandResult = await _mediator.Send(request);
                }

                return commandResult ? (IActionResult)Ok() : (IActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "  Details: " + ex.InnerException.ToString());
            }
        }


        // DELETE: api/Investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return Ok(investment);
        }
    }
}