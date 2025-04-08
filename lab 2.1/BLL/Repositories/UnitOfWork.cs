using BulletinBoard.DAL.Interfaces;
using BulletinBoard.DAL.Repositories;
using System;
using System.Threading.Tasks;

namespace BulletinBoard.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BulletinBoardContext _context;
        private IUserRepository _userRepository;
        private IAdRepository _adRepository;
        private ICategoryRepository _categoryRepository;
        private ITagRepository _tagRepository;
        private bool _disposed = false;

        public UnitOfWork(BulletinBoardContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IAdRepository Ads => _adRepository ??= new AdRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public ITagRepository Tags => _tagRepository ??= new TagRepository(_context);

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