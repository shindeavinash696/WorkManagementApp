using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementApp.Models.ViewModels
{
    public class ProductVM
    {
       public TaskDetails TaskDetails { get; set; }
        public IEnumerable<SelectListItem> ProjectList { get; set; }
        [NotMapped]
        public List<IFormFile> Files { get; set; } // Collection to hold multiple files
    }
}
