using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using DBTester.wwwroot.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrgxPublicApiSDK;

namespace DBTester.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private Context _context;

        public ToolsController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Tools()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(IFormFile file)
        {
            // SDK Test and it works 
            //var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");
            //var productInfo = listingApiClient.GetProductById("540250");

            if (file == null || file.Length == 0)
                return null;

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //Helper.ExcelGenerator(path);

            UPC upc = Helper.UPCLoadDic();

            //UPC upc = new UPC();

            //upc.UpcID = 1;
            //upc.Upc = 123456;

            if (ModelState.IsValid)
            {
                _context.UPC.Add(upc);
                _context.SaveChanges();
            }
                

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            FileStreamResult returnFile =
                File(memory, Helper.GetContentType(path), Path.GetFileNameWithoutExtension(path)
                + "_Converted" + Path.GetExtension(path).ToLowerInvariant());

            System.IO.File.Delete(path);

            return returnFile;
        }
    }
}