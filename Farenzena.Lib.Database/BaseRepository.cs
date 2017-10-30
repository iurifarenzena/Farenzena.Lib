using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database
{
    public abstract class BaseRepository<TPoco> : IRepository<TPoco> where TPoco : class
    {
        public bool Any()
        {
            return AsQueryable().Any();
        }

        public bool Any(Expression<Func<TPoco, bool>> filter)
        {
            return AsQueryable().Any(filter);
        }
        public abstract void ApplyChanges();
        public abstract IQueryable<TPoco> AsQueryable();

        public int Count()
        {
            return AsQueryable().Count();
        }

        public int Count(Expression<Func<TPoco, bool>> filter)
        {
            return AsQueryable().Count(filter);
        }

        public void DeleteAll()
        {
            DeleteAll(e => true);
        }

        public abstract void DeleteAll(Expression<Func<TPoco, bool>> filter);

        public async Task DeleteAllAsync()
        {
            await Task.Run(() => DeleteAll());
        }

        public async Task DeleteAllAsync(Expression<Func<TPoco, bool>> filter)
        {
            await Task.Run(() => DeleteAll(filter));
        }

        public abstract void Delete(TPoco entry);
        public abstract void Dispose();

        public IEnumerable<TPoco> GetAll()
        {
            return GetAll(e => true);
        }

        public IEnumerable<TPoco> GetAll(Expression<Func<TPoco, bool>> filter)
        {
            return AsQueryable().Where(filter);
        }
        public abstract IEnumerable<TPoco> GetAllLocal(Expression<Func<TPoco, bool>> filter);

        public TPoco Get(Expression<Func<TPoco, bool>> filter)
        {
            return AsQueryable().SingleOrDefault(filter);
        }

        public abstract TPoco Get(params object[] primaryKeys);
        public abstract TPoco GetLocal(Expression<Func<TPoco, bool>> filter);
        public abstract void Save(TPoco entry, bool forceInsert = false);
    }
}
