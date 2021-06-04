using Mego.Domain.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mego.Domain.Infrustructure.Metrics
{
    public sealed class MetricsRepository : IMetricsRepository
    {
        private readonly List<Metric> _metrics;

        public MetricsRepository()
        {
            _metrics = new List<Metric>();
        }

        public async Task<IEnumerable<Metric>> GetAll(CancellationToken cancellationToken)
        {
            return await Task.FromResult(_metrics);
        }

        public Task Save(Metric metric)
        {
            _metrics.Add(metric);

            return Task.CompletedTask;
        }
    }
}
