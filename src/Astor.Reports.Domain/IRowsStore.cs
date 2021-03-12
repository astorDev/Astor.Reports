using System.Collections.Generic;
using System.Threading.Tasks;

namespace Astor.Reports.Domain
{
    public interface IRowsStore
    {
        Task<int> CountAsync(string filter);

        Task AddAsync(IEnumerable<dynamic> rows);
    }
}