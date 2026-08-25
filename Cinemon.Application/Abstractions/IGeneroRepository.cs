using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IGeneroRepository
    {
        Task<bool> ExistAllAsync(
            IReadOnlyCollection<int> generoIds,
            CancellationToken cancellationToken);
    }
}
