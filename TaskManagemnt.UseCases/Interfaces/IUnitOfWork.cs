namespace TaskManagemnt.UseCases.Interfaces
{
    public  interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
