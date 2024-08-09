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
    public class SubTask
    {
        [Key]
        public int SubTaskId { get; set; }
        public string SubTaskName { get; set; }
        public string? Image { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MoneyIn { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MoneyOut { get; set; }
        public int TaskID { get; set; }
        public string Status { get; set; }

        [ForeignKey("TaskID")]
        [ValidateNever]
        public TaskDetails Task { get; set; }

        public int ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        [ValidateNever]
        public Projects Projects { get; set; }

        
    }
}
