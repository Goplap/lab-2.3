using BulletinBoard.DAL.Interfaces;
using BulletinBoard.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BulletinBoard.DAL
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly DbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;

        public RepositoryFactory(DbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repository = new EfGenericRepository<T>(_dbContext);
                _repositories.Add(type, repository);
            }

            return (IGenericRepository<T>)_repositories[type];
        }
    }
}