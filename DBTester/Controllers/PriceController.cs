using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using DBTester.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using OfficeOpenXml;
using FrgxPublicApiSDK;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using FastMember;
using System.Data;
using FrgxPublicApiSDK.Models;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ExcelModifier;

namespace DBTester.Controllers
{
    public class PriceController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;

        public PriceController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult PriceUpdater()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp.LastOrDefault().TimeStamp.ToShortDateString();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductImport(IFormFile file, int items)
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

            // Update the database once a day
            //updateFragrancex();

            //Dictionary<string, double> calculation = new Dictionary<string, double>();

            //Match shippingMatch = Regex.Match(shipping, @"[\d]+");

            //calculation.Add("shipping", Double.Parse(shippingMatch.Value));

            //Match amazonFee = Regex.Match(fee, @"[\d]+[/.]?[\d]+");

            //calculation.Add("fee", Double.Parse(amazonFee.Value));

            //Match profitMatch = Regex.Match(profit, @"[\d]+");

            //calculation.Add("profit", Double.Parse(profitMatch.Value));

            //if (markdown != null)
            //{
            //    Match markdownMatch = Regex.Match(markdown, @"[\d]+");
            //    calculation.Add("markDown", Double.Parse(markdownMatch.Value));
            //}

            //var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            var prices = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD);

            Profile profile = new Profile();

            IExcelExtension excelExtension = new ShopifyExcelUpdator(profile)
            {
                path = path,
                fragrancexPrices = prices
            };

            //ExcelHelper.ExcelGenerator(path, prices, items);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            FileStreamResult returnFile =
                File(memory, Helper.GetContentType(path), Path.GetFileNameWithoutExtension(path)
                + "_Price_Updated" + Path.GetExtension(path).ToLowerInvariant());

            System.IO.File.Delete(path);

            return returnFile;
        }
    }
}