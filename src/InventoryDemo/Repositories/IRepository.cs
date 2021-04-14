using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        ValueTask<TEntity> GetById(int id, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllReadOnly(CancellationToken cancellationToken = default);

        IAsyncEnumerable<TEntity> GetAllReadOnlyAsEnumerable();

        Task Add(TEntity entity, CancellationToken cancellationToken = default);

        Task Delete(TEntity Entity, CancellationToken cancellationToken = default);

        Task Edit(TEntity entity, CancellationToken cancellationToken = default);
    }
}
