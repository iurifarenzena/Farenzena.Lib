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
        bool Any(Func<TPoco, bool> filter);
        void ApplyChanges();
        IQueryable<TPoco> AsQueryable();
        int Count();
        int Count(Func<TPoco, bool> filter);
        void Delete(TPoco entry);
        void DeleteAll();
        void DeleteAll(Func<TPoco, bool> filter);
        Task DeleteAllAsync();
        Task DeleteAllAsync(Func<TPoco, bool> filter);
        TPoco Get(Func<TPoco, bool> filter);
        TPoco Get(params object[] primaryKeys);
        TPoco GetLocal(Func<TPoco, bool> filter);
        IEnumerable<TPoco> GetAll();
        IEnumerable<TPoco> GetAll(Func<TPoco, bool> filter);
        IEnumerable<TPoco> GetAllLocal(Func<TPoco, bool> filter);
        //IQueryable<TPoco> Where(Func<TPoco, bool> filter);
        void Save(TPoco entry, bool forceInsert = false);
    }
}
