using BulletinBoard.DAL.Interfaces;

namespace BulletinBoard.DAL.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly BulletinBoardContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private bool _disposed = false;

        public EfUnitOfWork(BulletinBoardContext context)
        {
            _context = context;
            _repositoryFactory = new EfRepositoryFactory(context);
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return _repositoryFactory.GetRepository<T>();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}