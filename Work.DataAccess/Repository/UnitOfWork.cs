using WorkManagementApp.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public INewProjectRepository Projects { get; private set; }
        public ITaskDetailsRepository TaskDetails { get; private set; }
        public ISubTasksRepository SubTasks { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Projects = new NewProjectRepository(_db);
            TaskDetails = new TaskDetailsRepository(_db);
            SubTasks = new SubTasksRepository(_db);
            ApplicationUsers = new ApplicationUserRepository(_db);
        }
       

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
