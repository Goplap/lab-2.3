using BulletinBoard.DAL.Interfaces;
using lab_2._1.DAL.Interfaces;
using System;
using System.Threading.Tasks;

namespace lab_2._1.DAL.Interfaces
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