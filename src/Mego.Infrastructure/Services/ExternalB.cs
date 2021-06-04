using Mego.Domain.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mego.Domain.Infrastructure.Services
{
    public class ExternalB : IExternal
    {
        private readonly TimeSpan _maxSearchDuration;
        private readonly IMetricsRepository _metricsService;

        public ExternalB(IOptions<SearchServiceOptions> options,
            IMetricsRepository metricsService)
        {
            _metricsService = metricsService;
            _maxSearchDuration = TimeSpan.FromSeconds(options.Value.MaxSearchDurationInSeconds);
        }

        public async Task<Result> Request(CancellationToken cancellationToken)
        {
            Metric metric = new Metric(
                Guid.NewGuid(),
                nameof(ExternalB),
                RandomRequest.RandomResult(),
                RandomRequest.RandomTimeRequest());

            if (metric.TimeRequest < _maxSearchDuration)
                await _metricsService.Save(metric);

            await Task.Delay(metric.TimeRequest, cancellationToken);

            return metric.Result;
        }
    }
}
