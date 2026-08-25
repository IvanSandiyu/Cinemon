using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(
            CancellationToken cancellationToken);

        Task CommitTransactionAsync(
            CancellationToken cancellationToken);

        Task RollbackTransactionAsync(
            CancellationToken cancellationToken);

        Task SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}
