using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DatabaseModifier;
using DBTester.Code;
using DBTester.Models;
using ExcelModifier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult UpdateExcel()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.PerfumeWorldWide.ToString())
                .LastOrDefault()?.TimeStamp.ToShortDateString();

            ViewBag.type = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.PerfumeWorldWide.ToString())
                .LastOrDefault()?.type;

            ViewBag.Wholesalers = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.PerfumeWorldWide.ToString())
                .LastOrDefault()?.Wholesalers;

            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            return View(_context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.PerfumeWorldWide.ToString())
                .OrderByDescending(x => x.TimeStamp).Take(5).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CompareExcel(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            fragrancex = _context.FragrancexTitle.ToDictionary(x => x.ItemID, y => y.Title);

            fragrancexUpc = _context.Fragrancex.Where(z => z.Upc != null).ToDictionary(x => x.ItemID, y => y.Upc);

            PerfumeWorldWideComparer perfumeWorldWideComparer = new PerfumeWorldWideComparer(fragrancex)
            {
                path = path,
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

        [HttpPost]
        public IActionResult UpdateDatabase(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            PerfumeWorldWide = _context.PerfumeWorldWide.ToDictionary(x => x.sku, y => y);

            DBModifierPerfumeWorldWideExcel dBModifierPerfumeWorldWideExcel 
                = new DBModifierPerfumeWorldWideExcel(path, PerfumeWorldWide);

            dBModifierPerfumeWorldWideExcel.TableExecutor();

            ServiceTimeStamp service = new ServiceTimeStamp();

            service.TimeStamp = DateTime.Today;

            service.Wholesalers = Wholesalers.PerfumeWorldWide.ToString();

            service.type = "Excel";

            _context.ServiceTimeStamp.Add(service);

            _context.SaveChanges();

            System.IO.File.Delete(path);

            return RedirectToAction("UpdateExcel");
        }

        private Dictionary<int, string> fragrancex { get; set; }

        private Dictionary<int, long?> fragrancexUpc { get; set; }

        private Dictionary<string, PerfumeWorldWide> PerfumeWorldWide { get; set; }
    }
}