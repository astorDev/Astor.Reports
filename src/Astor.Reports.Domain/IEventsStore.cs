using System.Threading.Tasks;

namespace Astor.Reports.Domain
{
    public interface IEventsStore
    {
        Task SaveAsync(EventChanges changes);
    }
}