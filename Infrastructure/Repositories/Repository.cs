using Domain.RepositoryInterfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<List<TEntity>> Get()
        {
            //return _dbContext.Set<TEntity>().ToListAsync();
            return _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetAll, {typeof(TEntity).Name}").ToListAsync();
        }
        public Task<List<TEntity>> GetActive()
        {
            //return _dbContext.Set<TEntity>().ToListAsync();
            return _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetActive, {typeof(TEntity).Name}").ToListAsync();
        }
        public Task<TEntity> GetFirst()
        {
            //return _dbContext.Set<TEntity>().ToListAsync();
            var entity = _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetFirst, {typeof(TEntity).Name}").ToList().FirstOrDefault();
            return Task.FromResult(entity);
        }
        public Task<TEntity> Get(int id)
        {
            //return _dbContext.Set<TEntity>().FindAsync(id).AsTask();
            var entity = _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetOne, {typeof(TEntity).Name}, {id}").ToList().FirstOrDefault();
            return Task.FromResult(entity);
        }
        public Task<TEntity> Get(string id)
        {
            //return _dbContext.Set<TEntity>().FindAsync(id).AsTask();
            var entity = _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetOne, {typeof(TEntity).Name}, {id}").ToList().FirstOrDefault();
            return Task.FromResult(entity);
        }
        public Task<List<TEntity>> GetMultiple(string ids)
        {
            //return _dbContext.Set<TEntity>().FindAsync(id).AsTask();
            return _dbContext.Set<TEntity>().FromSqlRaw($"spCommonProc GetMultiple, {typeof(TEntity).Name}, {ids}").ToListAsync();
        }
        public void Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }
        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
        public Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
        {
            return _dbContext.Set<TEntity>().FirstOrDefaultAsync(filter);
        }
        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter)
        {
            return _dbContext.Set<TEntity>().Where(filter).ToListAsync();
        }
        public void CreateRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().AddRange(entities);
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
        }
        public EntityEntry<TEntity> GetEntityEntry(TEntity entity)
        {
            return _dbContext.Entry(entity);
        }
        public EntityState GetEntityState(TEntity entity)
        {
            return _dbContext.Entry(entity).State;
        }
        public void SetEntityState(TEntity entity, EntityState state)
        {
            _dbContext.Entry(entity).State = state;
        }
        public Task<List<TEntity>> GetListFromSql(string procName, List<KeyValuePair<string, string>> values)
        {
            var parameters = new object[values.Count];
            for (var i = 0; i < values.Count; i++)
                parameters[i] = new SqlParameter(values[i].Key, values[i].Value);
            var paramnames = values.Aggregate("", (current, item) => current + item.Key + ",");
            paramnames = paramnames.TrimEnd(',');
            procName = procName + " " + paramnames;
            return _dbContext.Set<TEntity>().FromSqlRaw(procName, parameters).ToListAsync();
        }
        public int GetFlagFromSql(string procName, List<KeyValuePair<string, string>> values)
        {
            values.Add(new KeyValuePair<string, string>("@ReturnValue", ""));
            var parameters = new object[values.Count];
            var flagParam = new SqlParameter { ParameterName = "@ReturnValue", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            for (var i = 0; i < values.Count; i++)
            {
                parameters[i] = new SqlParameter(values[i].Key, values[i].Value);
                if (values[i].Key == "@ReturnValue")
                    parameters[i] = flagParam;
            }
            var paramnames = values.Aggregate("", (current, item) =>
            {
                if (item.Key == "@ReturnValue")
                    return current + item.Key + " output,";
                return current + item.Key + ",";
            });
            paramnames = paramnames.TrimEnd(',');
            procName = "exec " + procName + " " + paramnames;
            _dbContext.Database.ExecuteSqlRaw(procName, parameters);
            var Flag = Convert.ToInt32(flagParam.Value);
            return Flag;
        }
    }
}
