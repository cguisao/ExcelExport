using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using Microsoft.AspNetCore.Mvc;

namespace GTI_Solutions.Controllers
{
    public class AzImporterController : Controller
    {
        public Context _context;

        public AzImporterController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}