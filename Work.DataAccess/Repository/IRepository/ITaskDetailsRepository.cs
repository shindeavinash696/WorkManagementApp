using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Repository.IRepository
{
    public interface ITaskDetailsRepository : IRepository<TaskDetails>
    {
        void Update(TaskDetails obj);
        TaskDetails GetFirstOrDefault(Expression<Func<TaskDetails, bool>> filter, string includeProperties = null);
    }
}
