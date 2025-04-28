namespace BulletinBoard.DAL.Interfaces
{
    public interface IRepositoryFactory
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
    }
}