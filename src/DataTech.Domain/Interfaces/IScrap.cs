using DataTech.Domain.Common;
using System.Threading.Tasks;

namespace DataTech.Domain.Interfaces
{
    public interface IScrap<In, Out>
    {
        Task<Result<Out>> Scrap(In param);
    }
}