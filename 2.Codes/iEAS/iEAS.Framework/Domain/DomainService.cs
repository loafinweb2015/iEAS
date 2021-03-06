﻿using iEAS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace iEAS
{
    public class DomainService
    {
        /// <summary>
        /// 从Repository中获取返回值
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <param name="layzLoad"></param>
        /// <returns></returns>
        public static TResult Fetch<TRepository, TResult>(Func<TRepository, TResult> handler, bool layzLoad = false)
            where TRepository : BaseRepository, new()
        {
            if (layzLoad)
            {
                var rep = ObjectContainer.GetService<TRepository>();
                return handler(rep);
            }
            else
            {
                using (var rep = new TRepository())
                {
                    return handler(rep);
                }
            }
        }

        /// <summary>
        /// 在Repository中执行操作，无返回值
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="handler"></param>
        public static void Execute<TRepository>(Action<TRepository> handler)
            where TRepository : BaseRepository, new()
        {
            using (var rep = new TRepository())
            {
                handler(rep);
            }
        }
    }

    public class DomainService<TRepository> : DomainService
        where TRepository:BaseRepository,new()
     {
        /// <summary>
        /// 从Repository中获取数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="handler"></param>
        /// <param name="layzLoad"></param>
        /// <returns></returns>
         public TResult Fetch<TResult>(Func<TRepository, TResult> handler, bool layzLoad = false)
         {
             return DomainService.Fetch<TRepository, TResult>(handler, layzLoad);
         }

        /// <summary>
        /// 在Repository中执行数据操作
        /// </summary>
        /// <param name="handler"></param>
         public void Execute(Action<TRepository> handler)
         {
             DomainService.Execute<TRepository>(handler);
         }
     }

     public class DomainService<TEntity, TRepository>
        :DomainService<TRepository>,IDomainService<TEntity,TRepository> 
        where TRepository:BaseRepository,new()
        where TEntity:BaseEntity
    {
         /// <summary>
         /// 创建实体
         /// </summary>
         /// <param name="entity"></param>
        public virtual void Create(TEntity entity)
        {
            Execute(rep => rep.Create(entity));
        }

         /// <summary>
         /// 批量创建实体
         /// </summary>
         /// <param name="items"></param>
        public virtual void Create(IEnumerable<TEntity> items)
        {
            Execute(rep => rep.Create<TEntity>(items));
        }

         /// <summary>
         /// 更新实体
         /// </summary>
         /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            Execute(rep => rep.Update(entity));
        }

         /// <summary>
         /// 批量更新实体
         /// </summary>
         /// <param name="predicate"></param>
         /// <param name="handler"></param>
        public virtual void Update(Expression<Func<TEntity,bool>> predicate,Action<TEntity> handler)
        {
            Execute(rep =>rep.Update(predicate, handler));
        }

         /// <summary>
         /// 按ID更新实体
         /// </summary>
         /// <param name="id"></param>
         /// <param name="handler"></param>
        public void UpdateByID(int id,Action<TEntity> handler)
        {
            Update(m => m.ID == id, handler);
        }

         /// <summary>
         /// 按Guid更新实体
         /// </summary>
         /// <param name="guid"></param>
         /// <param name="handler"></param>
        public void UpdateByGuid(Guid guid,Action<TEntity> handler)
        {
            Update(m => m.Guid == guid, handler);
        }

         /// <summary>
         /// 创建或更新实体
         /// </summary>
         /// <param name="entity"></param>
        public void CreateOrUpdate(TEntity entity)
        {
            var orign=GetByID(entity.ID);
            if (orign != null)
            {
                Update(entity);
            }
            else
            {
                Create(entity);
            }
        }

         /// <summary>
         /// 删除实体
         /// </summary>
         /// <param name="entity"></param>
        public virtual void Delete(TEntity entity)
        {
            Execute(rep => rep.Delete<TEntity>(entity));
        }

         /// <summary>
         /// 按条件批量删除实体
         /// </summary>
         /// <param name="predicate"></param>
        public virtual void Delete(Expression<Func<TEntity,bool>> predicate)
        {
            Execute(rep => rep.Delete<TEntity>(predicate));
        }

         /// <summary>
         /// 按ID删除实体
         /// </summary>
         /// <param name="id"></param>
        public void DeleteByID(int id)
        {
            Delete(m => m.ID == id);
        }

         /// <summary>
         /// 按Guid删除实体
         /// </summary>
         /// <param name="guid"></param>
        public void DeleteByGuid(Guid guid)
        {
            Delete(m => m.Guid == guid);
        }

         /// <summary>
         /// 按Id获取实体
         /// </summary>
         /// <param name="id"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public TEntity GetByID(int id, bool lazyLoad = false)
        {
            return Get(m => m.ID == id, lazyLoad);
        }

         /// <summary>
         /// 按Guid获取实体
         /// </summary>
         /// <param name="guid"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public TEntity GetByGuid(Guid guid, bool lazyLoad = false)
        {
            return Get(m => m.Guid == guid, lazyLoad);
        }

         /// <summary>
         /// 按查询条件获取实体
         /// </summary>
         /// <param name="predicate"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate, bool lazyLoad = false)
        {
            return Fetch<TEntity>(rep => rep.Get<TEntity>(predicate), lazyLoad);
        }

         /// <summary>
         /// 列表查询
         /// </summary>
         /// <param name="predicate"></param>
         /// <param name="orderBy"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public virtual IList<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> orderBy = null, bool lazyLoad = false)
        {
            return Fetch<IList<TEntity>>(rep =>
            {
                return rep.Query<TEntity>(predicate, orderBy).ToList();
            }
                , lazyLoad);
        }

         /// <summary>
         /// 列表查询
         /// </summary>
         /// <param name="orderBy"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public IList<TEntity> Query(Action<Orderable<TEntity>> orderBy = null, bool lazyLoad = false)
        {
            return Query(null, orderBy, lazyLoad);
        }

         /// <summary>
         /// 列表查询
         /// </summary>
         /// <param name="predicate"></param>
         /// <param name="orderBy"></param>
         /// <param name="startRow"></param>
         /// <param name="maxRows"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public virtual IList<TEntity> Query(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> orderBy, int startRow, int maxRows, bool lazyLoad = false)
        {
            return Fetch<IList<TEntity>>(rep =>
            {
                return rep.Query<TEntity>(predicate, orderBy, startRow, maxRows).ToList();
            }
            , lazyLoad);
        }

         /// <summary>
         /// 列表查询
         /// </summary>
         /// <param name="orderBy"></param>
         /// <param name="startRow"></param>
         /// <param name="maxRows"></param>
         /// <param name="lazyLoad"></param>
         /// <returns></returns>
        public IList<TEntity> Query(Action<Orderable<TEntity>> orderBy, int startRow, int maxRows, bool lazyLoad = false)
        {
            return Query(null, orderBy, startRow, maxRows, lazyLoad);
        }

         /// <summary>
         /// 分页查询
         /// </summary>
         /// <param name="predicate"></param>
         /// <param name="orderBy"></param>
         /// <param name="pageIndex"></param>
         /// <param name="pageSize"></param>
         /// <returns></returns>
        public virtual PagedResult<TEntity> PagedQuery(Expression<Func<TEntity,bool>> predicate,Action<Orderable<TEntity>> orderBy,int pageIndex,int pageSize)
        {
            return Fetch(rep => rep.PagedQuery<TEntity>(predicate, orderBy, pageIndex, pageSize));
        }

         /// <summary>
         /// 分页查询
         /// </summary>
         /// <param name="orderBy"></param>
         /// <param name="pageIndex"></param>
         /// <param name="pageSize"></param>
         /// <returns></returns>
        public PagedResult<TEntity> PagedQuery(Action<Orderable<TEntity>> orderBy, int pageIndex, int pageSize)
        {
            return PagedQuery(null,orderBy,pageIndex,pageSize);
        }
    }

     public class IdentityDomainService<TEntity, TRepository>
        : DomainService<TEntity,TRepository>, IDomainService<TEntity, TRepository>
             where TRepository : BaseRepository, new()
             where TEntity : IdentityEntity
     {
         public override void Create(IEnumerable<TEntity> items)
         {
             foreach(var item in items)
             {
                 item.Creator = AppContext.Current.User.Number;
                 item.CreateTime = DateTime.Now;
                 item.Updator = AppContext.Current.User.Number;
                 item.UpdateTime = DateTime.Now;
             }
             base.Create(items);
         }

         public override void Create(TEntity entity)
         {
             entity.Creator = AppContext.Current.User.Number;
             entity.CreateTime = DateTime.Now;
             entity.Updator = AppContext.Current.User.Number;
             entity.UpdateTime = DateTime.Now;
             base.Create(entity);
         }

         public override void Delete(Expression<Func<TEntity, bool>> predicate)
         {
             this.Update(predicate, m =>
             {
                 m.Status=2;
             });
         }

         public override void Delete(TEntity entity)
         {
             entity.Status = 2;
             this.Update(entity);
         }

         public override void Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> handler)
         {
             Update(predicate, m =>
             {
                 m.UpdateTime = DateTime.Now;
                 m.Updator = AppContext.Current.User.Number;
                 handler(m);
             });
         }

         public override void Update(TEntity entity)
         {
             entity.Updator = AppContext.Current.User.Number;
             entity.UpdateTime = DateTime.Now;
             base.Update(entity);
         }
     }
}
