using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Repository.IRepository
{

    public interface ISubTasksRepository : IRepository<SubTask>
    {
        void Update(SubTask obj);
    }
}
