using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Repository
{
    public class TaskDetailsRepository : Repository<TaskDetails>, ITaskDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public TaskDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(TaskDetails obj)
        {
            var objFromDb = _db.TaskDetail.FirstOrDefault(u => u.TaskId == obj.TaskId);
            if (objFromDb != null)
            {
                objFromDb.TaskName = obj.TaskName;
                objFromDb.UpdatedBy = obj.UpdatedBy;
                objFromDb.TaskBuget = obj.TaskBuget;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }

        public TaskDetails GetFirstOrDefault(Expression<Func<TaskDetails, bool>> filter, string includeProperties = null)
        {
            IQueryable<TaskDetails> query = _db.TaskDetail;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }
    }
}
