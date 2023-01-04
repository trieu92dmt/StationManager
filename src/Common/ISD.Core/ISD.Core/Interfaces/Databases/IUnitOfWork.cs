namespace ISD.Core.Interfaces.Databases
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task Dispose();
    }
}
