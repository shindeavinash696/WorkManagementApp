using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementApp.Models
{
    public class TaskDetails
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string TaskName { get; set; }

        public int TaskBuget { get; set; }
        public string? UpdatedBy { get; set; }

        public int ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        [ValidateNever]
        public Projects Projects { get; set; }

        public string ImageUrl { get; set; } 

        public ICollection<SubTask> SubTasks { get; set; }  // Ensure this property exists
    }

}
