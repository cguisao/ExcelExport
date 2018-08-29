using DatabaseModifier;
using DBTester.Code;
using DBTester.Models;
using ExcelModifier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DBTester.Controllers
{
    public class AmazonController : Controller
    {
        public Context _context;

        public AmazonController(Context context)
        {
            _context = context;
        }
        
        public IActionResult Upload()
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

            ViewBag.amazonItems = _context.Amazon.Count();

            ViewBag.amazonFragrancex = _context.Amazon.Where(x => x.wholesaler == Wholesalers.Fragrancex.ToString()).Count();

            ViewBag.amazonAzImporter = _context.Amazon.Where(x => x.wholesaler == Wholesalers.AzImporter.ToString()).Count();

            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            Profile profile = new Profile();

            return View(_context.Profile.ToList());
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
        public async Task<IActionResult> UpdateAmazonDB(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            // Put the code here

            SetDictionariesAsync();

            amazonList = _context.Amazon.ToList();

            //amazonItems = amazonList.ToDictionary(x => x.Asin, y => y.sku);

            //amazonList2 = _context.Amazon.ToList();

            var tasks = new List<Task>();

            //Task amazonListTask = new Task(() => amazonList = _context.Amazon.ToList());

            Task amazonItemsTask = new Task(() => amazonItems = amazonList.ToDictionary(x => x.Asin, y => y.sku));

            Task amazonList2Task = new Task(() => amazonList2 = _context.Amazon.ToList());

            //tasks.Add(amazonListTask);

            tasks.Add(amazonItemsTask);

            tasks.Add(amazonList2Task);

            Parallel.ForEach(tasks, task =>
            {
                task.RunSynchronously();
            });

            AmazonDBUploader amazonDBUploader = new AmazonDBUploader(amazonItems, amazonList, amazonList2)
            {
                path = path,
                fragrancexPrices = fragrancexPrices,
                azImportPrice = azImportPrice,
                azImportQuantity = azImportQuantity,
                ShippingtWeight = shippingWeight,
                azImporterWeightSku = azImporterWeightSku
            };
            
            try
            {
                amazonDBUploader.ExcelGenerator();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                _context.Database.ExecuteSqlCommand("delete from Amazon");

                // Upload to the DB

                DBModifierAmazon dBModifierAmazon = new DBModifierAmazon(amazonDBUploader.amazonList);

                dBModifierAmazon.TableExecutor();
            }
            
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

        [HttpPost]
        public async Task<IActionResult> UpdateAmazonList(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            SetDictionariesAsync();
            
            AmazonExcelUpdator amazonExcelUpdator = new AmazonExcelUpdator()
            {
                path = path,
                fragrancexPrices = fragrancexPrices,
                azImportPrice = azImportPrice,
                azImportQuantity = azImportQuantity,
                ShippingtWeight = shippingWeight,
                azImporterWeightSku = azImporterWeightSku
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

        private void SetDictionariesAsync()
        {
            var azImporter = _context.AzImporter.ToHashSet();

            var fragrancex = _context.Fragrancex.ToHashSet();

            shippingWeight = _context.Shipping.ToDictionary(x => x.weightId, y => y.ItemPrice);
            
            var tasks = new List<Task>();

            Task fragrancexPricesTask = new Task(() => fragrancexPrices = fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD));

            Task azImportPriceTask = new Task(() => azImportPrice = azImporter.ToDictionary(x => x.Sku, y => y.WholeSale));

            Task azImportQuantityTask = new Task(() => azImportQuantity = azImporter.ToDictionary(x => x.Sku, y => y.Quantity));

            Task azImporterWeightSkuTask = new Task(() => azImporterWeightSku = azImporter.ToDictionary(x => x.Sku, y => y.Weight));

            tasks.Add(fragrancexPricesTask);

            tasks.Add(azImportPriceTask);

            tasks.Add(azImportQuantityTask);

            tasks.Add(azImporterWeightSkuTask);

            Parallel.ForEach(tasks, task =>
            {
                task.RunSynchronously();
            });
        }

        private List<Amazon> amazonList { get; set; }

        private Dictionary<int, double> fragrancexPrices { get; set; }

        private Dictionary<string, double> azImportPrice { get; set; }

        private Dictionary<string, int> azImportQuantity { get; set; }

        private Dictionary<string, int> azImporterWeightSku { get; set; }

        private Dictionary<int, double> shippingWeight { get; set; }

        private Dictionary<string, string> amazonItems { get; set; }

        private List<Amazon> amazonList2 { get; set; }
    }
}
