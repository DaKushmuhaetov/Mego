using Mego.Domain.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mego.Domain.Infrastructure.Services
{
    public class ExternalD : IExternal
    {
        private readonly IMetricsRepository _metricsService;

        public ExternalD(IMetricsRepository metricsService)
        {
            _metricsService = metricsService;
        }

        public async Task<Result> Request(CancellationToken cancellationToken)
        {
            Metric metric = new Metric(
                Guid.NewGuid(),
                nameof(ExternalD),
                RandomRequest.RandomResult(),
                RandomRequest.RandomTimeRequest());

            await Task.Delay(metric.TimeRequest, cancellationToken);

            await _metricsService.Save(metric);

            return metric.Result;
        }
    }
}
