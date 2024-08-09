using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Repository.IRepository
{
    public interface INewProjectRepository:IRepository<Projects>
    {
        void Update(Projects obj);
    }
}
