using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Repositories.Infrastructure.Repositories
{
    public interface IGeneRepo<TEntity>
    {
        #region Get
        IQueryable<TEntity> GetQuery();
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression);
        DbContext GetDbContext();

        TEntity Single(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria);


        #endregion

        #region Find
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);
        TEntity FindOne(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> criteria);
        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>>[] includeExpressions);
        #endregion

        #region Count

        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> criteria);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria);
        Task<bool> GetAny(Expression<Func<TEntity, bool>> expression);
        #endregion

        #region Update
        void Update(TEntity entity, Expression<Func<TEntity, bool>> criteria);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region Add
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        #endregion

        #region Remove
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        #endregion

        #region Sql
        #endregion
        // Get the lastest value from db
        void Reload(TEntity entity);

    }
}
