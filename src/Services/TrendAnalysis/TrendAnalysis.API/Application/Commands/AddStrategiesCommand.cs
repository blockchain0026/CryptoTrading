using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoTrading.Services.TrendAnalysis.API.Application.Commands
{
    public class AddStrategiesCommand : IRequest<bool>
    {
        public AddStrategiesCommand(string traceId, string[] strategies)
        {
            TraceId = traceId;
            Strategies = strategies;
        }

        [DataMember]
        public string TraceId { get; private set; }
        [DataMember]
        public string[] Strategies { get; private set; }

    }
}
