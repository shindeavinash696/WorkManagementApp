using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Diagnostics;
using WorkManagementApp.DataAccess.Repository.IRepository;
using WorkManagementApp.Models;
using WorkManagementApp.Utility;

namespace WorkManagementApp.Areas.LocalUser.Controllers
{
    [Area("LocalUser")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [Authorize(Roles = SD.Role_user_Admin + "," + SD.Role_user_Staff)]
        public IActionResult ProjectView()
        {
            // Get all projects
            var projects = _unitOfWork.Projects.GetAll().ToList();

            // Initialize a list to hold the project details with calculated profit and loss
            var projectDetails = new List<dynamic>();

            foreach (var project in projects)
            {
                // Fetch tasks associated with the current project
                var tasks = _unitOfWork.TaskDetails.GetAll(u => u.ProjectID == project.ProjectId, includeProperties: "SubTasks").ToList();


                var totalRemainingTaskBudget = tasks.Sum(t => t.TaskBuget - (t.SubTasks.Sum(s => s.MoneyIn)) - t.SubTasks.Sum(s => s.MoneyOut));

                // Calculate total remaining task budget for the current project
               // var totalRemainingTaskBudget = tasks.Sum(t => t.TaskBuget - t.SubTasks.Sum(s => s.MoneyOut));

                // Calculate profit and loss
                var profitAndLoss = project.Budget - totalRemainingTaskBudget;

                // Add project details to the list
                projectDetails.Add(new
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    Budget = project.Budget,
                    TotalRemainingTaskBudget = totalRemainingTaskBudget,
                    ProfitAndLoss = profitAndLoss
                });
            }

            // Pass the data to the view using ViewBag
            ViewBag.ProjectDetails = projectDetails;

            return View();
        }



        [Authorize(Roles = SD.Role_user_Admin + "," + SD.Role_user_Staff)]
        public IActionResult TaskView(int id)
        {
            var tasks = _unitOfWork.TaskDetails.GetAll(u => u.ProjectID == id, includeProperties: "SubTasks,Projects").ToList();

            var remainingBudgets = new Dictionary<int, decimal>();

            foreach (var task in tasks)
            {
                var totalSubTaskMoneyIn = task.SubTasks.Sum(s => (decimal?)s.MoneyIn) ?? 0;
                var totalSubTaskMoneyOut = task.SubTasks.Sum(s => (decimal?)s.MoneyOut) ?? 0;
                var TotalSubTaskMoney = totalSubTaskMoneyIn - totalSubTaskMoneyOut;
                var remainingBudget = task.TaskBuget - TotalSubTaskMoney;
                remainingBudgets[task.TaskId] = remainingBudget;
            }

            ViewBag.RemainingBudgets = remainingBudgets;

            return View(tasks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
