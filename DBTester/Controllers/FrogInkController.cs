using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DatabaseModifier;
using DBTester.Code;
using DBTester.Models;
using ExcelModifier;
using FrgxPublicApiSDK;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GTI_Solutions.Controllers
{
    public class FrogInkController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;

        public FrogInkController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        
        public IActionResult Index()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault().TimeStamp.ToShortDateString();

            ViewBag.type = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault().type;

            ViewBag.Wholesalers = _context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault().Wholesalers;

            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            var profile = new Profile();

            return View(_context.Profile.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(string file, string shipping
            , string fee, string profit, string promoting, string markdown, int items, int min
            , int max, string User)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            // Update the database once a day
            updateFragrancex();

            Match shippingMatch = Regex.Match(shipping, @"[\d]+");

            Match amazonFee = Regex.Match(fee, @"[\d]+[/.]?[\d]+");

            Match promotingFee = Regex.Match(promoting, @"[\d]+[/.]?[\d]+");

            Match profitMatch = Regex.Match(profit, @"[\d]+");

            Profile oldProfile = _context.Profile.AsNoTracking().Where<Profile>(x => x.ProfileUser == User).FirstOrDefault();

            Profile profile = new Profile
            {
                shipping = Double.Parse(shippingMatch.Value),

                fee = Double.Parse(amazonFee.Value),

                profit = Double.Parse(profitMatch.Value),

                promoting = Double.Parse(promotingFee.Value),

                ProfileUser = User,

                items = items,

                min = min,

                max = max,

                html = oldProfile.html,

                LongstartTitle = oldProfile.LongstartTitle,

                MidtartTitle = oldProfile.MidtartTitle,

                ShortstartTitle = oldProfile.ShortstartTitle,

                endTtile = oldProfile.endTtile,

                sizeDivider = oldProfile.sizeDivider

            };

            if (markdown != null)
            {
                Match markdownMatch = Regex.Match(markdown, @"[\d]+");
                profile.markdown = Double.Parse(markdownMatch.Value);
            }

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            var prices = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD);

            var descriptions = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.Description);
            
            FrogInkExcelCreator frogInkExcelCreator = new FrogInkExcelCreator(upc, profile)
            {
                path = path,
                fragrancexPrices = prices,
                descriptions = descriptions
            };

            frogInkExcelCreator.ExcelGenerator();

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            FileStreamResult returnFile =
                File(memory, Helper.GetContentType(path), profile.ProfileUser
                + "_Converted_" + DateTime.Today.GetDateTimeFormats()[10]
                + Path.GetExtension(path).ToLowerInvariant());

            _context.Profile.Update(profile);

            _context.SaveChanges();

            System.IO.File.Delete(path);

            return returnFile;
        }

        private void updateFragrancex()
        {
            ServiceTimeStamp service = new ServiceTimeStamp();

            if (_context.ServiceTimeStamp.Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault<ServiceTimeStamp>() == null)
            {
                FragancexSQLPreparer(service);
            }
            else if (_context.ServiceTimeStamp.Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
            {
                FragancexSQLPreparer(service);
            }
        }

        private void FragancexSQLPreparer(ServiceTimeStamp service)
        {
            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            // TODO: Test this functionality once Alex is done with his development!!

            try
            {
                var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");

                // For testing purposes

                //List<Product> allProducts = new List<Product>();

                //var product = listingApiClient.GetProductById("412492");

                //allProducts.Add(product);

                var allProducts = listingApiClient.GetAllProducts();

                DBModifierFragrancexAPI dBModifierFragrancexAPI = new DBModifierFragrancexAPI("", upc)
                {
                    allProducts = allProducts
                };

                dBModifierFragrancexAPI.TableExecutor();

                service.TimeStamp = DateTime.Today;

                service.Wholesalers = Wholesalers.Fragrancex.ToString();

                service.type = "API";

                _context.ServiceTimeStamp.Add(service);

                _context.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }
    }
}