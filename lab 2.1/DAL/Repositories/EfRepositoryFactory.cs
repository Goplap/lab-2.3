using BulletinBoard.DAL.Interfaces;

namespace BulletinBoard.DAL.Repositories
{
    public class EfRepositoryFactory : IRepositoryFactory
    {
        private readonly DbContext _context;

        public EfRepositoryFactory(DbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new EfGenericRepository<T>(_context);
        }
    }
}