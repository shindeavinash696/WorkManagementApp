using WorkManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementApp.DataAccess.Repository.IRepository
{

    public interface IApplicationUserRepository: IRepository<ApplicationUser>
    {
        public void Update(ApplicationUser application);
    }
}
