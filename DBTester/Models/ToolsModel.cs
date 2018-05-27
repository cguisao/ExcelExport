using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Models
{
    public class ToolsModel : PageModel
    {
        public ToolsModel() { }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Excel application.";
        }
    }
}
