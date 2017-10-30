using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database.EFCore
{
    public class EFCoreBaseRepository<TPoco> : BaseRepository<TPoco> where TPoco : class
    {
        private DbContext _context;

        public EFCoreBaseRepository(DbContext contexto)
        {
            _context = contexto;
        }

        public override void ApplyChanges()
        {
            _context.SaveChanges();
        }

        public override IQueryable<TPoco> AsQueryable()
        {
            return _context.Set<TPoco>();
        }

        public override void Delete(TPoco entry)
        {
            var dbentry = ToDbEntry(entry);
            if (dbentry.IsKeySet)
            {
                if (dbentry.State == EntityState.Detached)
                    _context.Attach(entry).State = EntityState.Deleted;
                else
                    _context.Set<TPoco>().Remove(entry);
            }
        }

        public override void DeleteAll(Expression<Func<TPoco, bool>> filter)
        {
            var allItems = _context.Set<TPoco>().Where(filter);
            _context.Set<TPoco>().RemoveRange(allItems);
        }

        public override void Dispose()
        {
            _context = null;
        }
        
        public override TPoco Get(params object[] primaryKeys)
        {
            return _context.Set<TPoco>().Find(primaryKeys);
        }

        public override IEnumerable<TPoco> GetAllLocal(Expression<Func<TPoco, bool>> filter)
        {
            return _context.Set<TPoco>().Local.Where(filter.Compile());
        }

        public override TPoco GetLocal(Expression<Func<TPoco, bool>> filter)
        {
            return _context.Set<TPoco>().Local.SingleOrDefault(filter.Compile());
        }

        public override void Save(TPoco entry, bool forceInsert = false)
        {
            Save(ToDbEntry(entry), forceInsert);
        }

        public void Save(EntityEntry<TPoco> dbentry, bool forceInsert)
        {
            if (forceInsert || !dbentry.IsKeySet)
            {
                var referencesToIgnore = dbentry.Members.OfType<Microsoft.EntityFrameworkCore.ChangeTracking.ReferenceEntry>()
                    .Where(refer => refer.CurrentValue != null)
                    .Select(refer => _context.Entry(refer.CurrentValue))
                    .Where(e => e.State == EntityState.Detached && e.IsKeySet).ToList();

                foreach (var reference in referencesToIgnore)
                {
                    _context.Attach(reference.Entity).State = EntityState.Unchanged;
                }

                _context.Set<TPoco>().Add(dbentry.Entity as TPoco);
            }
            else if (dbentry.State == EntityState.Detached || // Não está atachado
                (dbentry.State != EntityState.Added && dbentry.State != EntityState.Deleted)) // Ou não está marcado para deletar ou adicionar
            {
                _context.Attach(dbentry.Entity).State = EntityState.Modified;
            }
        }

        private Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TPoco> ToDbEntry(TPoco registro)
        {
            return _context.Entry(registro);
        }
    }
}
