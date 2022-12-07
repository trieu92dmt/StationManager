using ISD.API.EntityModels.Data;
using ISD.API.Repositories.Infrastructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISD.API.Repositories.Infrastructure.Database
{
    public class ISDUnitOfWork : IISDUnitOfWork
    {
        private readonly EntityDataContext _context;
        private bool disposed = false;

        public ISDUnitOfWork(EntityDataContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public IGeneRepo<T> AsyncRepository<T>() where T : class
        {
            return new GeneRepo<T>(_context);
        }

        private async Task Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }
            disposed = true;
        }

        public async Task Dispose()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
