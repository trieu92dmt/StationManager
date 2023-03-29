namespace Core.Interfaces.Databases
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task Dispose();
    }

    public interface IUnitOfWork<TContext>
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task Dispose();
    }
}
