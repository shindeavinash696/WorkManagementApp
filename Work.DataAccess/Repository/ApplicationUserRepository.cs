using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;
using WorkManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.DataAccess.Repository;

namespace WorkManagementApp.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>,IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db): base(db)
        {
                _db = db;
        }
        public void Update(ApplicationUser application)
        {
            _db.ApplicationUsers.Update(application);
        }
    }
}
