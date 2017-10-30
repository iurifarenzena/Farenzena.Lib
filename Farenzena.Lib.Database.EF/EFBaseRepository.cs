
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database.EF
{
    public class EFBaseRepository<TPoco> : BaseRepository<TPoco> where TPoco : class
    {
        private DbContext _context;

        public EFBaseRepository(DbContext contexto)
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

        public override void DeleteAll(Expression<Func<TPoco, bool>> filter)
        {
            var allItems = _context.Set<TPoco>().Where(filter);
            _context.Set<TPoco>().RemoveRange(allItems);
        }

        public override void Delete(TPoco entry)
        {
            var dbentry = ToDbEntry(entry);
            if (dbentry.IsKeySet(_context))
            {
                if (dbentry.State == EntityState.Detached)
                {
                    _context.Set<TPoco>().Attach(entry);
                    dbentry.State = EntityState.Deleted;
                }
                else
                    _context.Set<TPoco>().Remove(entry);
            }
        }

        public override void Dispose()
        {
            _context = null;
        }

        public override IEnumerable<TPoco> GetAllLocal(Expression<Func<TPoco, bool>> filter)
        {
            return _context.Set<TPoco>().Local.Where(filter.Compile());
        }

        public override TPoco Get(params object[] primaryKeys)
        {
            return _context.Set<TPoco>().Find(primaryKeys);
        }

        public override TPoco GetLocal(Expression<Func<TPoco, bool>> filter)
        {
            return _context.Set<TPoco>().Local.SingleOrDefault(filter.Compile());
        }

        public override void Save(TPoco entry, bool forceInsert = false)
        {
            Save(ToDbEntry(entry), forceInsert);
        }

        public void Save(DbEntityEntry<TPoco> entry, bool forceInsert)
        {
            if (forceInsert || !entry.IsKeySet(_context))
            {
                //var referencesToIgnore = entry.CurrentValues.PropertyNames.Select(pname => entry.ComplexProperty(pname)).Where(prop => prop != null)
                //    .Where(refer => refer.CurrentValue != null)
                //    .Select(refer => _context.Entry(refer.CurrentValue))
                //    .Where(e => e.State == EntityState.Detached && e.IsKeySet(_context)).ToList();

                //foreach (var reference in referencesToIgnore)
                //{
                //    _context.Set<TPoco>().Attach(entry.Entity);
                //    reference.State = EntityState.Unchanged;
                //}

                _context.Set<TPoco>().Add(entry.Entity as TPoco);
            }
            else if (entry.State != EntityState.Modified &&
                (entry.State == EntityState.Detached || // Não está atachado
                (entry.State != EntityState.Added && entry.State != EntityState.Deleted))) // Ou não está marcado para deletar ou adicionar
            {
                _context.Set<TPoco>().Attach(entry.Entity);
                entry.State = EntityState.Modified;
            }
        }

        private DbEntityEntry<TPoco> ToDbEntry(TPoco registro)
        {
            return _context.Entry(registro);
        }
    }
}
