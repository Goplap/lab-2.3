using BulletinBoard.DAL.Interfaces;
using lab_2._1.DAL.Interfaces;
using lab_2._1.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly BulletinBoardContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private bool _disposed = false;

        // Add repository fields
        private IUserRepository _userRepository;
        private IAdRepository _adRepository;
        private ICategoryRepository _categoryRepository;
        private ITagRepository _tagRepository;

        public EfUnitOfWork(BulletinBoardContext context)
        {
            _context = context;
            _repositoryFactory = new RepositoryFactory(context);
        }

        // Implement repository properties
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IAdRepository Ads => _adRepository ??= new AdRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public ITagRepository Tags => _tagRepository ??= new TagRepository(_context);

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