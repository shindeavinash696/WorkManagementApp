using Microsoft.AspNetCore.Mvc;
using WorkManagementApp.Models;
using WorkManagementApp.DataAccess.Data;
using WorkManagementApp.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WorkManagementApp.Utility;
namespace WorkManagementApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_user_Admin)]
    public class ProjectsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Projects> objProjectList = _unitOfWork.Projects.GetAll().ToList();
            return View(objProjectList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Projects obj)
        {
            _unitOfWork.Projects.Add(obj);
            _unitOfWork.Save();
            TempData["Success"] = obj.ProjectName + " project created successfully !!";
            return RedirectToAction("Index", "Projects");
        }
        public IActionResult Edit(int? projectId)
        {
            if (projectId == null || projectId == 0)
            {
                return NotFound();
            }
            Projects newProjectFromDb = _unitOfWork.Projects.Get(u => u.ProjectId == projectId);
            // NewProject newProjectFromDb1 = _db.NewProjects.FirstOrDefault(u=>u.ProjectId==projectId);
            // NewProject newProjectFromDb1 = _db.NewProjects.Where(u => u.ProjectId==projectId).FirstOrDefault();
            if (newProjectFromDb == null)
            {
                return NotFound();
            }
            return View(newProjectFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Projects obj)
        {
            _unitOfWork.Projects.Update(obj);
            _unitOfWork.Save();
            TempData["Success"] = obj.ProjectName + " project updated successfully !!";
            return RedirectToAction("Index", "Projects");
        }
        public IActionResult Delete(int? projectId)
        {
            if (projectId == null || projectId == 0)
            {
                return NotFound();
            }
            Projects newProjectFromDb = _unitOfWork.Projects.Get(u => u.ProjectId == projectId);
            // NewProject newProjectFromDb1 = _db.NewProjects.FirstOrDefault(u=>u.ProjectId==projectId);
            // NewProject newProjectFromDb1 = _db.NewProjects.Where(u => u.ProjectId==projectId).FirstOrDefault();
            if (newProjectFromDb == null)
            {
                return NotFound();
            }
            return View(newProjectFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? projectId)
        {
            Projects? obj = _unitOfWork.Projects.Get(u => u.ProjectId == projectId);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Projects.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = obj.ProjectName + " project deleted successfully !!";
            return RedirectToAction("Index", "Projects");
        }
    }
}
