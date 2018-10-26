using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DBTester.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DBTester.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult test()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DropzoneFileUpload(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            if (!file.FileName.Contains(".xlsx"))
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Wrong file type");
                return null;
            }

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        fileName + ".xlsx");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
