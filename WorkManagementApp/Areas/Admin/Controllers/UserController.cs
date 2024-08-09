using Microsoft.AspNetCore.Mvc;
using WorkManagementApp.Models;
using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;
using System.Threading.Tasks;
using WorkManagementApp.Models.ViewModels;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using WorkManagementApp.Utility;
using Microsoft.EntityFrameworkCore;
using WorkManagementApp.DataAccess.Repository;
namespace WorkManagementApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_user_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _webHostEnvironment=webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

       

        #region API CALLS
        [HttpGet]
        public ActionResult GetAll()
        {
            List<ApplicationUser> objTaskList = _unitOfWork.ApplicationUsers.GetAll().ToList();

            return Json(new { Data = objTaskList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.ApplicationUsers.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.ApplicationUsers.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation successfully" });
        }

        #endregion
    }
}
