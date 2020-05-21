using DataTech.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataTech.Domain.Interfaces
{
    public interface IProvider<In, Out>
    {
        Task<Result<Out>> Flow(In param, CancellationToken ct);
    }
}