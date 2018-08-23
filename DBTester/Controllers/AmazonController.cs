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
            ViewBag.TimeStampFragrancex = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault()?.TimeStamp.ToShortDateString();

            ViewBag.typeAzFragrancex = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault()?.type;

            ViewBag.TimeStampAzImport = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .LastOrDefault()?.TimeStamp.ToShortDateString();

            ViewBag.typeAzImport = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .LastOrDefault()?.type;

            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            Profile profile = new Profile();

            return View(_context.Profile.ToList());
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
        public async Task<IActionResult> UpdateAmazonList(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            //var listTest = _context.AzImporter.ToHashSet();

            var fragrancexPrices = _context.Fragrancex.ToDictionaryAsync(x => x.ItemID, y => y.WholePriceUSD);

            var azImportPrice = _context.AzImporter.ToDictionaryAsync(x => x.Sku, y => y.WholeSale);
            
            var azImportQuantity = _context.AzImporter.ToDictionaryAsync(x => x.Sku, y => y.Quantity);

            var ShippingWeight = _context.Shipping.ToDictionaryAsync(x => x.weightId, y => y.ItemPrice);

            var azImporterWeightSku = _context.AzImporter.ToDictionaryAsync(x => x.Sku, y => y.Weight);
            
            //var azImport = _context.AzImporter.ToDictionaryAsync(x => x.Sku, listTest);


            AmazonExcelUpdator amazonExcelUpdator = new AmazonExcelUpdator()
            {
                path = path,
                fragrancexPrices = await fragrancexPrices,
                azImportPrice = await azImportPrice,
                azImportQuantity = await azImportQuantity,
                ShippingtWeight = await ShippingWeight,
                azImporterWeightSku = await azImporterWeightSku
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

            System.IO.File.Delete(path);

            return returnFile;
        }
    }
}
