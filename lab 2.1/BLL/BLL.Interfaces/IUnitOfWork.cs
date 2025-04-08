using System;
using System.Threading.Tasks;

namespace BulletinBoard.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IAdRepository Ads { get; }
        ICategoryRepository Categories { get; }
        ITagRepository Tags { get; }
        Task SaveChangesAsync();
    }
}