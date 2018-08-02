using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DBTester.Code;
using DBTester.Models;
using ExcelModifier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBTester.Controllers
{
    public class AmazonController : Controller
    {
        public Context _context;

        public AmazonController(Context context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp.LastOrDefault().TimeStamp.ToShortDateString();

            ViewBag.type = _context.ServiceTimeStamp.LastOrDefault().type;

            Profile profile = new Profile();

            return View(_context.Profile.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAmazonList(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var prices = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD);
            
            AmazonExcelUpdator amazonExcelUpdator = new AmazonExcelUpdator()
            {
                sWebRootFolder = path,
                prices = prices
            };

            amazonExcelUpdator.ExcelGenerator();

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            FileStreamResult returnFile =
                File(memory, Helper.GetContentType(path), "Amazon"
                + "_Converted_" + DateTime.Today.GetDateTimeFormats()[10]
                + Path.GetExtension(path).ToLowerInvariant());

            //_context.Profile.Update(profile);

            //_context.SaveChanges();

            System.IO.File.Delete(path);

            return returnFile;
        }
    }
}
