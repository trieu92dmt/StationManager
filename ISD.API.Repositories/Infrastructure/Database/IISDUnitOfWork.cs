using System.Threading.Tasks;

namespace ISD.API.Repositories.Infrastructure.Database
{
    public interface IISDUnitOfWork
    {
        int SaveChanges();

        Task<int> SaveChangesAsync();
        Task Dispose();
    }
}
