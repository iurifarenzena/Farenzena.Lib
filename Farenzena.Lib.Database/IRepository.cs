using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database
{
    public interface IRepository<TPoco> : IDisposable where TPoco : class
    {
        bool Any();
        bool Any(Expression<Func<TPoco, bool>> filter);
        void ApplyChanges();
        IQueryable<TPoco> AsQueryable();
        IQueryable<TPoco> AsQueryableNoTracking();
        int Count();
        int Count(Expression<Func<TPoco, bool>> filter);
        void Delete(TPoco entry);
        void DeleteAll();
        void DeleteAll(Expression<Func<TPoco, bool>> filter);
        Task DeleteAllAsync();
        Task DeleteAllAsync(Expression<Func<TPoco, bool>> filter);
        TPoco Get(Expression<Func<TPoco, bool>> filter);
        TPoco Get(params object[] primaryKeys);
        TPoco GetLocal(Expression<Func<TPoco, bool>> filter);
        IEnumerable<TPoco> GetAll();
        IEnumerable<TPoco> GetAllNoTracking();
        IEnumerable<TPoco> GetAll(Expression<Func<TPoco, bool>> filter);
        IEnumerable<TPoco> GetAllNoTracking(Expression<Func<TPoco, bool>> filter);
        IEnumerable<TPoco> GetAllLocal(Expression<Func<TPoco, bool>> filter);
        //IQueryable<TPoco> Where(Func<TPoco, bool> filter);
        void Save(TPoco entry, bool forceInsert = false);
    }
}
