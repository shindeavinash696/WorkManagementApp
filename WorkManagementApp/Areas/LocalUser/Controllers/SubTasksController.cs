using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using System.Diagnostics;
using WorkManagementApp.DataAccess.Repository.IRepository;
using WorkManagementApp.Models;
using WorkManagementApp.Utility;

namespace WorkManagementApp.Areas.LocalUser.Controllers
{
    [Area("LocalUser")]

    [Authorize(Roles = SD.Role_user_Admin + "," + SD.Role_user_Staff)]
    public class SubTasksController : Controller
    {
        private readonly ILogger<SubTasksController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SubTasksController(ILogger<SubTasksController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public static class FileHelper
        {
            public static string GetFileExtension(string fileName)
            {
                return Path.GetExtension(fileName)?.ToLower();
            }
        }


        public IActionResult Index(int id, string searchString)
        {
            var task = _unitOfWork.TaskDetails.GetFirstOrDefault(t => t.TaskId == id, includeProperties: "SubTasks");

            if (task == null)
            {
                return NotFound();
            }

            IEnumerable<SubTask> subTaskList = _unitOfWork.SubTasks.GetAll(u => u.TaskID == id, includeProperties: "Task,Projects");

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                subTaskList = subTaskList.Where(s => s.SubTaskName.ToLower().Contains(searchString) ||
                                                     s.Task.TaskName.ToLower().Contains(searchString) ||
                                                     s.Status.ToLower().Contains(searchString) ||
                                                     s.Projects.ProjectName.ToLower().Contains(searchString));
            }

            ViewBag.SumMoneyIn = subTaskList.Sum(s => s.MoneyIn);
            ViewBag.SumMoneyOut = subTaskList.Sum(s => s.MoneyOut);
            ViewBag.TotalSubTaskBudget = ViewBag.SumMoneyIn - ViewBag.SumMoneyOut;
            ViewBag.RemainingTaskBudget = task.TaskBuget - ViewBag.TotalSubTaskBudget;

            return View(subTaskList);
        }


        // GET: SubTasks/Details/5
        public IActionResult Details(int subTaskId)
        {
            var subTask = _unitOfWork.SubTasks.GetFirstOrDefault(s => s.SubTaskId == subTaskId, includeProperties: "Task,Projects");
            if (subTask == null)
            {
                return NotFound();
            }

            return View(subTask);
        }

        public IActionResult Create()
        {
            var tasks = _unitOfWork.TaskDetails.GetAll().Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName
            });

            var projects = _unitOfWork.Projects.GetAll().Select(p => new SelectListItem
            {
                Value = p.ProjectId.ToString(),
                Text = p.ProjectName
            });

            ViewBag.Tasks = tasks;
            ViewBag.Projects = projects;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubTask obj, IFormFileCollection uploadedFiles)
        {
            if (!ModelState.IsValid)
            {
                // Log or inspect validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log error.ErrorMessage or inspect it for debugging
                    Console.WriteLine(error.ErrorMessage);
                }

                // Re-populate dropdowns in case of error
                var tasks = _unitOfWork.TaskDetails.GetAll().Select(t => new SelectListItem
                {
                    Value = t.TaskId.ToString(),
                    Text = t.TaskName
                });

                var projects = _unitOfWork.Projects.GetAll().Select(p => new SelectListItem
                {
                    Value = p.ProjectId.ToString(),
                    Text = p.ProjectName
                });

                ViewBag.Tasks = tasks;
                ViewBag.Projects = projects;

                return View(obj);
            }

            if (uploadedFiles != null && uploadedFiles.Count > 0)
            {
                // Get the task based on TaskID
                var task = _unitOfWork.TaskDetails.GetFirstOrDefault(t => t.TaskId == obj.TaskID);
                var taskName = task?.TaskName;

                if (!string.IsNullOrEmpty(taskName))
                {
                    // Define the folder path for uploads
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", taskName);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    foreach (var uploadedFile in uploadedFiles)
                    {
                        if (uploadedFile.Length > 0)
                        {
                            // Define the file path
                            var filePath = Path.Combine(folderPath, uploadedFile.FileName);

                            // Save the file to the file system
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(stream);
                            }

                            // Store the relative path to the file in the database
                            // Replace backslashes with forward slashes for URL compatibility
                            var filePathForDb = Path.Combine("uploads", taskName, uploadedFile.FileName).Replace('\\', '/');

                            // Append the file path to the Image property
                            obj.Image += filePathForDb + ";"; // Assuming semicolon as delimiter
                        }
                    }

                    // Remove the last semicolon
                    if (!string.IsNullOrEmpty(obj.Image))
                    {
                        obj.Image = obj.Image.TrimEnd(';');
                    }
                }
            }

            _unitOfWork.SubTasks.Add(obj);
            _unitOfWork.Save();
            TempData["Success"] = obj.SubTaskName + " created successfully !!";
            return RedirectToAction("ProjectView", "Home");
        }


        //------------------------------------------------------edit---------------------------------------------------------
        // GET: SubTasks/Edit/5
        public IActionResult Edit(int subTaskId)
        {
            var subTask = _unitOfWork.SubTasks.GetFirstOrDefault(s => s.SubTaskId == subTaskId);
            if (subTask == null)
            {
                return NotFound();
            }

            var tasks = _unitOfWork.TaskDetails.GetAll().Select(t => new SelectListItem
            {
                Value = t.TaskId.ToString(),
                Text = t.TaskName
            }).ToList();

            var projects = _unitOfWork.Projects.GetAll().Select(p => new SelectListItem
            {
                Value = p.ProjectId.ToString(),
                Text = p.ProjectName
            }).ToList();

            ViewBag.Tasks = tasks;
            ViewBag.Projects = projects;

            return View(subTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubTask obj, List<IFormFile> uploadedFiles)
        {
            try
            {
                if (obj == null)
                {
                    return BadRequest("SubTask object cannot be null");
                }
                if (!ModelState.IsValid)
                {
                    var tasks = _unitOfWork.TaskDetails.GetAll().Select(t => new SelectListItem
                    {
                        Value = t.TaskId.ToString(),
                        Text = t.TaskName
                    });

                    var projects = _unitOfWork.Projects.GetAll().Select(p => new SelectListItem
                    {
                        Value = p.ProjectId.ToString(),
                        Text = p.ProjectName
                    });

                    ViewBag.Tasks = tasks;
                    ViewBag.Projects = projects;

                    return View(obj);
                }

                var existingSubTask = _unitOfWork.SubTasks.GetFirstOrDefault(s => s.SubTaskId == obj.SubTaskId);
                if (existingSubTask == null)
                {
                    return NotFound();
                }

                if (uploadedFiles != null && uploadedFiles.Count > 0)
                {
                    var task = _unitOfWork.TaskDetails.GetFirstOrDefault(t => t.TaskId == obj.TaskID);
                    var taskName = task?.TaskName;

                    if (!string.IsNullOrEmpty(taskName))
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", taskName);

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (var file in uploadedFiles)
                        {
                            var filePath = Path.Combine(folderPath, file.FileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            if (string.IsNullOrEmpty(existingSubTask.Image))
                            {
                                existingSubTask.Image = Path.Combine("uploads", taskName, file.FileName).Replace('\\', '/');
                            }
                            else
                            {
                                existingSubTask.Image += ";" + Path.Combine("uploads", taskName, file.FileName).Replace('\\', '/');
                            }
                        }
                    }
                }

                existingSubTask.SubTaskName = obj.SubTaskName;
                existingSubTask.MoneyIn = obj.MoneyIn;
                existingSubTask.MoneyOut = obj.MoneyOut;
                existingSubTask.TaskID = obj.TaskID;
                existingSubTask.ProjectID = obj.ProjectID;
                existingSubTask.Status = obj.Status;

                _unitOfWork.SubTasks.Update(existingSubTask);
                _unitOfWork.Save();
                TempData["Success"] = obj.SubTaskName + " updated successfully!";
                return RedirectToAction("ProjectView", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                TempData["Error"] = "An error occurred: " + ex.Message;
                return View(obj);
            }
        }

        [HttpPost]
        public IActionResult DeleteFile(int subTaskId, string filePath)
        {
            var subTask = _unitOfWork.SubTasks.GetFirstOrDefault(s => s.SubTaskId == subTaskId);
            if (subTask == null)
            {
                return NotFound();
            }

            var filePaths = subTask.Image?.Split(';').ToList();
            if (filePaths != null && filePaths.Contains(filePath))
            {
                filePaths.Remove(filePath);
                subTask.Image = string.Join(";", filePaths);

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Replace('/', '\\'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                _unitOfWork.SubTasks.Update(subTask);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Edit), new { subTaskId });
        }


        //---------------------------------------------------delete-------------------------------------------

        // GET: SubTasks/Delete/5
        public IActionResult Delete(int subTaskId)
        {
            var subTask = _unitOfWork.SubTasks.GetFirstOrDefault(u => u.SubTaskId == subTaskId, includeProperties: "Task,Projects");
            if (subTask == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(subTask.Image))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subTask.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            ViewBag.Tasks = _unitOfWork.TaskDetails.GetAll().Select(t => new { Value = t.TaskId, Text = t.TaskName });
            ViewBag.Projects = _unitOfWork.Projects.GetAll().Select(p => new { Value = p.ProjectId, Text = p.ProjectName });

            return View(subTask);
        }

        // POST: SubTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int subTaskId)
        {
            var subTask = _unitOfWork.SubTasks.GetFirstOrDefault(u => u.SubTaskId == subTaskId);
            if (subTask == null)
            {
                return NotFound();
            }

            _unitOfWork.SubTasks.Remove(subTask);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
 
    }
}
