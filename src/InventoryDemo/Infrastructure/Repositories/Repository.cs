using InventoryDemo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : BaseEntity where TContext : DbContext
    {
        protected readonly TContext _context;

        public Repository(TContext context) => _context = context;

        public ValueTask<TEntity> GetById(int id, CancellationToken cancellationToken = default) =>
            _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);

        public IAsyncEnumerable<TEntity> GetAllReadOnlyAsEnumerable() =>
            _context.Set<TEntity>().AsNoTracking().AsAsyncEnumerable();

        public Task<List<TEntity>> GetAllReadOnly(CancellationToken cancellationToken = default) =>
            _context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

        public async Task Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).Property(c => c.Deleted).IsModified = true;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Edit(TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
