using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Code;
using DBTester.Models;
using ExcelModifier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBTester.Controllers
{
    public class PerfumeWorldWideController : Controller
    {
        public Context _context;

        public PerfumeWorldWideController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .LastOrDefault()?.TimeStamp.ToShortDateString();

            ViewBag.type = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .LastOrDefault()?.type;

            ViewBag.Wholesalers = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .LastOrDefault()?.Wholesalers;

            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            return View(_context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .OrderByDescending(x => x.TimeStamp).Take(5).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> DropzoneFileUpload(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
            {
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

        [HttpPost]
        public async Task<IActionResult> CompareExcel(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            //var fragrancexPrices = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD);

            var fragrancexUpc = _context.Fragrancex.Where(z => z.Upc != null).ToDictionary(x => x.ItemID, y => y.Upc);

            PerfumeWorldWideComparer perfumeWorldWideComparer = new PerfumeWorldWideComparer()
            {
                path = path,
                //fragrancexPrices = fragrancexPrices,
                fragrancexUpc = fragrancexUpc
            };

            perfumeWorldWideComparer.ExcelGenerator();

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