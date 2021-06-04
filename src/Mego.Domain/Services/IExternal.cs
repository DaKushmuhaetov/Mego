using System.Threading;
using System.Threading.Tasks;

namespace Mego.Domain.Services
{
    public interface IExternal
    {
        Task<Result> Request(CancellationToken cancellationToken);
    }
}
