using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Farenzena.Lib.Database.EF
{
    public static class DbEntityEntryExtension
    {
        private static readonly Dictionary<Type, object> _commonTypeDictionary = new Dictionary<Type, object>
        {
#pragma warning disable IDE0034 // Simplify 'default' expression - default causes default(object)
            { typeof(int), default(int) },
            { typeof(Guid), default(Guid) },
            { typeof(DateTime), default(DateTime) },
            { typeof(DateTimeOffset), default(DateTimeOffset) },
            { typeof(long), default(long) },
            { typeof(bool), default(bool) },
            { typeof(double), default(double) },
            { typeof(short), default(short) },
            { typeof(float), default(float) },
            { typeof(byte), default(byte) },
            { typeof(char), default(char) },
            { typeof(uint), default(uint) },
            { typeof(ushort), default(ushort) },
            { typeof(ulong), default(ulong) },
            { typeof(sbyte), default(sbyte) }
#pragma warning restore IDE0034 // Simplify 'default' expression
        };

        public static object GetDefaultValue(this Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
            {
                return null;
            }

            // A bit of perf code to avoid calling Activator.CreateInstance for common types and
            // to avoid boxing on every call. This is about 50% faster than just calling CreateInstance
            // for all value types.
            return _commonTypeDictionary.TryGetValue(type, out var value)
                ? value
                : Activator.CreateInstance(type);
        }

        public static bool IsKeySet<TPoco>(this DbEntityEntry<TPoco> entry, DbContext dbContext) where TPoco : class
        {
            return IsKeySet((DbEntityEntry)entry, dbContext);

            //return !entry.FindPrimaryKeys(dbContext).Any(
            //    p => p.GetValue(entry) != null && p.GetValue(entry).Equals(p.DeclaringType.GetDefaultValue())
            //);
        }


        public static bool IsKeySet(this DbEntityEntry entry, DbContext dbContext)
        {
            return !entry.GetKeyProperties(dbContext)?.Any(kpr =>
                kpr.DefaultValue == entry.Property(kpr.Name).CurrentValue
            ) ?? false;


            //return !entry.FindPrimaryKeys(dbContext).Any(
            //    p => p.Value!=null && p.Value.Equals(p.Value.GetType().GetDefaultValue())
            //);
        }


        public static IEnumerable<PropertyInfo> FindPrimaryKeys<TPoco>(this DbEntityEntry<TPoco> entry, DbContext dbContext) where TPoco : class
        {
            var type = typeof(TPoco);

            var set = ((IObjectContextAdapter)dbContext).ObjectContext.CreateObjectSet<TPoco>();
            var entitySet = set.EntitySet;
            var keys = entitySet.ElementType.KeyMembers;
            return keys.Select(k => type.GetProperty(k.Name));
        }

        //public static EntityKeyMember[] FindPrimaryKeys(this DbEntityEntry entry, DbContext dbContext)
        //{
        //    return ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity).EntityKey.EntityKeyValues;
        //}

        private static IEnumerable<EdmProperty> GetKeyProperties(this DbEntityEntry entry, DbContext dbContext)
        {
            var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext;
            return GetKeyPropertyNames(entry.Entity.GetType(), objectContext.MetadataWorkspace);
        }

        private static IEnumerable<EdmProperty> GetKeyPropertyNames(Type type, MetadataWorkspace workspace)
        {
            EdmType edmType;

            if (!workspace.TryGetType(type.Name, type.Namespace, DataSpace.OSpace, out edmType))
                workspace.TryGetType(type.Name, type.Namespace, DataSpace.CSpace, out edmType);

            if (edmType != null)
            {
                return edmType.MetadataProperties.Where(mp => mp.Name == "KeyMembers")
                    .SelectMany(mp => mp.Value as ReadOnlyMetadataCollection<EdmMember>)
                    .OfType<EdmProperty>();
            }

            return null;
        }
    }
}
