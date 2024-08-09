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
    public class SubTasksRepository : Repository<SubTask>, ISubTasksRepository
    {
        private readonly ApplicationDbContext _db;

        public SubTasksRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SubTask obj)
        {
            _db.SubTasks.Update(obj);
        }
    }

}
