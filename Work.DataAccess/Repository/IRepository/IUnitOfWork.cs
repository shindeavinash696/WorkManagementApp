using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementApp.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        INewProjectRepository Projects { get; }
        ITaskDetailsRepository TaskDetails { get; }
        ISubTasksRepository SubTasks { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        void Save();
    }
}
