﻿using ePizzaHub.Core.Database;
using ePizzaHub.Repositories.Interfaces;

namespace ePizzaHub.Repositories.Impelmentations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected AppDbContext _db;
        public Repository(AppDbContext db)
        {
            _db = db;
        }

        public void Add(TEntity entity)
        {
            _db.Set<TEntity>().Add(entity);
        }

        public void Delete(object id)
        {
            TEntity entity = _db.Set<TEntity>().Find(id);
            if(entity != null)
            {
                this.Remove(entity);
            }
        }

        public TEntity Find(object id)
        {
            return _db.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _db.Set<TEntity>().ToList();
        }

        public void Remove(TEntity entity)
        {
            _db.Set<TEntity>().Remove(entity);
        }

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _db.Set<TEntity>().Update(entity);
        }
    }
}
