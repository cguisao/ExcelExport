using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopifyExcel.Pages.Code;

namespace ShopifyExcel
{
    public class ToolsModel : PageModel
    {
        public List<Technology> Technologies
        {
            get
            {
                return StaticData.Technologies;
            }
        }

        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Excel application.";
        }

    }
}
