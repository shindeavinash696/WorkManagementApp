using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;

namespace WorkManagementApp.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null) // Add this method
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }

}


//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using WorkManagementApp.DataAccess.Data;
//using WorkManagementApp.DataAccess.Repository.IRepository;

//namespace WorkManagementApp.DataAccess.Repository
//{
//    public class Repository<T> : IRepository<T> where T : class
//    {
//        private readonly ApplicationDbContext _db;
//        public DbSet<T> dbSet;
//        public Repository(ApplicationDbContext db)
//        {
//            _db = db;
//            this.dbSet = _db.Set<T>();
//            _db.TaskDetail.Include(u => u.Projects).Include(u=>u.ProjectID);


//        }
//        public void Add(T entity)
//        {
//            dbSet.Add(entity);
//        }

//        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
//        {
//            IQueryable<T> query = dbSet;
//            query = query.Where(filter);
//            if (!string.IsNullOrEmpty(includeProperties))
//            {
//                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    query = query.Include(property);
//                }
//            }
//            return query.FirstOrDefault();
//        }

//        //public IEnumerable<T> GetAll(string? includeProperties = null)
//        //{
//        //    IQueryable<T> query = dbSet;
//        //    if (!string.IsNullOrEmpty(includeProperties))
//        //    {
//        //        foreach (var property in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
//        //        {
//        //            query = query.Include(property);
//        //        }
//        //    }
//        //    return query.ToList();
//        //}
//        public IEnumerable<T> GetAll( Expression<Func<T, bool>> filter = null,string includeProperties = null)
//        {
//            IQueryable<T> query = dbSet;

//            // Apply the filter if provided
//            if (filter != null)
//            {
//                query = query.Where(filter);
//            }

//            // Include related properties if specified
//            if (!string.IsNullOrEmpty(includeProperties))
//            {
//                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    query = query.Include(property);
//                }
//            }

//            return query.ToList();
//        }


//        public void Remove(T entity)
//        {
//            dbSet.Remove(entity);
//        }

//        public void RemoveRange(IEnumerable<T> entity)
//        {
//            dbSet.RemoveRange(entity);
//        }
//    }
//}
