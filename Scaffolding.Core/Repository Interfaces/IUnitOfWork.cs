using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffolding.Core.Repository_Interfaces
{
    public interface IUnitOfWork : IReadOnlyUnitOfWork
    {
        /// <summary>
        /// Saves the changes for each registered context.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Saves the changes for each registered context.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Async task</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
