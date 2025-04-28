using BulletinBoard.DAL.Repositories;
using lab_2._1.DAL.Interfaces;

namespace BulletinBoard.DAL.Interfaces
{
    public interface IRepositoryFactory
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
    }
}