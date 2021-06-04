using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mego.Domain.Services
{
    public interface IMetricsRepository
    {
        Task Save(Metric metric);

        Task<IEnumerable<Metric>> GetAll(CancellationToken cancellationToken);
    }
}
