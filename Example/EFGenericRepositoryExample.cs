using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnitOfWork;

namespace Example
{
    class EFGenericRepositoryExample<T> : IGenericRepository<T> where T : class
    {
        private readonly IUnitOfWorkManager manager;

        public EFGenericRepositoryExample(IUnitOfWorkManager manager)
        {
            this.manager = manager;
        }

        public IQueryable<T> DataSet
        {
            get
            {
                var dbContext = manager.CurrentContext as DbContext;
                return dbContext.Set<T>();
            }
        }

        public void AddOrUpdate(T data)
        {
            var context = manager.CurrentContext as DbContext;
            var dbSet = context.Set<T>();
            var ids = context.Model
                .FindEntityType(typeof(T))
                .FindPrimaryKey()
                .Properties.Select(x => x.Name);

            var t = typeof(T);
            List<PropertyInfo> keyFields = new List<PropertyInfo>();

            foreach (var propt in t.GetProperties())
            {
                var keyAttr = ids.Contains(propt.Name);
                if (keyAttr)
                {
                    keyFields.Add(propt);
                }
            }
            if (keyFields.Count <= 0)
            {
                throw new Exception($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
            }
            ParameterExpression param1 = Expression.Parameter(t, "param1");
            Expression body = null;

            foreach (var keyField in keyFields)
            {
                var keyVal = keyField.GetValue(data);
                var prop = Expression.Property(param1, keyField);
                var eval = Expression.Equal(prop, Expression.Constant(keyVal));

                if(body != null)
                {
                    body = Expression.AndAlso(body, eval);
                }
                else
                {
                    body = eval;
                }
            }

            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(body, param1);
            var entities = dbSet.AsNoTracking();
            var dbVal = entities.FirstOrDefault(predicate);
            if (dbVal != null)
            {
                context.Entry(dbVal).CurrentValues.SetValues(data);
                context.Entry(dbVal).State = EntityState.Modified;
                return;
            }
            dbSet.Add(data);
        }
    }
}