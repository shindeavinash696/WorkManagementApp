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
namespace WorkManagementApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_user_Admin)]
    public class TasksController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TasksController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment=webHostEnvironment;
        }
       

        public IActionResult Index()
        {
            List<TaskDetails> objTaskList = _unitOfWork.TaskDetails.GetAll(includeProperties: "Projects").ToList();
            
            return View(objTaskList);
        }

        public IActionResult Upsert(int? taskId)
        {
            ProductVM taskVm = new()
            {
                ProjectList = _unitOfWork.Projects.GetAll().Select(u => new SelectListItem
                {
                    Text = u.ProjectName,
                    Value = u.ProjectId.ToString()
                }),

                TaskDetails = new TaskDetails()
            };

            var tasks = _unitOfWork.Projects.GetAll().Select(t => new SelectListItem
            {
                Value = t.SupervisorName,
                Text = t.SupervisorName,
            });
            ViewBag.SupervisorName = tasks;

            if (taskId == null || taskId == 0)
            {
                return View(taskVm);
            }
            else
            {
                taskVm.TaskDetails = _unitOfWork.TaskDetails.Get(u => u.TaskId == taskId);
                return View(taskVm);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            var tasks = _unitOfWork.Projects.GetAll().Select(t => new SelectListItem
            {
                Value = t.SupervisorName,
                Text = t.SupervisorName,
            });

            ViewBag.SupervisorName = tasks;

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string projectPath = Path.Combine(wwwRootPath, @"images\project");

                if (!string.IsNullOrEmpty(productVM.TaskDetails.ImageUrl))
                {
                    var oldImgPath = Path.Combine(wwwRootPath, productVM.TaskDetails.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }
                }

                using (var filestrem = new FileStream(Path.Combine(projectPath, fileName), FileMode.Create))
                {
                    file.CopyTo(filestrem);
                }
                productVM.TaskDetails.ImageUrl = @"\images\project\" + fileName;
            }

            if (productVM.TaskDetails.TaskId == 0)
            {
                _unitOfWork.TaskDetails.Add(productVM.TaskDetails);
            }
            else
            {
                _unitOfWork.TaskDetails.Update(productVM.TaskDetails);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Task created successfully !!";
            return RedirectToAction("Index", "Tasks");
        }



        public IActionResult Edit(int? taskId)
        {
            if (taskId == null || taskId == 0)
            {
                return NotFound();
            }
            TaskDetails newProjectFromDb = _unitOfWork.TaskDetails.Get(u => u.TaskId == taskId);
            // NewProject newProjectFromDb1 = _db.NewProjects.FirstOrDefault(u=>u.ProjectId==projectId);
            // NewProject newProjectFromDb1 = _db.NewProjects.Where(u => u.ProjectId==projectId).FirstOrDefault();
            if (newProjectFromDb == null)
            {
                return NotFound();
            }
            return View(newProjectFromDb);
        }
       
       

        #region API CALLS
        [HttpGet]
        public ActionResult GetAll()
        {
            List<TaskDetails> objTaskList = _unitOfWork.TaskDetails.GetAll(includeProperties: "Projects").ToList();
            return Json(new { Data = objTaskList });
        }

        [HttpDelete]
        public ActionResult Delete(int? id)
        {
            var taskDelete = _unitOfWork.TaskDetails.Get(u => u.TaskId == id);
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (taskDelete == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }
            var oldImgPath = Path.Combine(_webHostEnvironment.WebRootPath, taskDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImgPath))
            {
                System.IO.File.Delete(oldImgPath);
            }
            _unitOfWork.TaskDetails.Remove(taskDelete);
            _unitOfWork.Save();
            return Json(new { success = true, Message = "Task deleted successfully" });
        }

        #endregion
    }
}
