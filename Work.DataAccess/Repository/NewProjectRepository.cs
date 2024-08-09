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
    public class NewProjectRepository : Repository<Projects>, INewProjectRepository
    {
        private ApplicationDbContext _db;
        public NewProjectRepository(ApplicationDbContext db): base(db) 
        {
                _db = db;
        }
       
        public void Update(Projects obj)
        {
            _db.Projects.Update(obj);
        }
    }
}
