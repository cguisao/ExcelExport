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
using DatabaseModifier;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DBTester.Controllers
{
    public class ShopifyController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;
 
        public ShopifyController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            test = string.Empty;
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

            return View(_context.UsersList.Distinct().ToList());
        }

        public IActionResult Upload()
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

            return View(_context.Profile.ToList());
        }
        
        public IActionResult Update()
        {
            return View(_context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .OrderByDescending(x => x.TimeStamp).Take(5).ToList());
        }
        
        public IActionResult UpdateExcel()
        {
            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            return View(_context.ServiceTimeStamp
                .Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .OrderByDescending(x => x.TimeStamp).Take(5).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ProductExport(IFormFile file)
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

            ServiceTimeStamp service = new ServiceTimeStamp();

            if (_context.ServiceTimeStamp.Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault<ServiceTimeStamp>() == null)
            {
                Helper.tablePreparer(path);

                service.TimeStamp = DateTime.Today;
                service.Wholesalers = Wholesalers.Fragrancex.ToString();
                _context.ServiceTimeStamp.Add(service);
                _context.SaveChanges();

            }
            else if(_context.ServiceTimeStamp.Where(x => x.Wholesalers == Wholesalers.Fragrancex.ToString())
                .LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
            {
                Helper.tablePreparer(path);
                service.Wholesalers = Wholesalers.Fragrancex.ToString();
                service.TimeStamp = DateTime.Today;
                _context.ServiceTimeStamp.Add(service);
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


        [HttpPost]
        public IActionResult Upload(string file, string User, Profile profile2)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            Profile profile = _context.Profile.AsNoTracking().Where<Profile>(x => x.ProfileUser == User).FirstOrDefault();

            var shopifyProfile = _context.ShopifyUser.ToDictionary(x => x.sku, x => x);
            var shopifyUsers = _context.UsersList.Where(x => x.userID == profile.ProfileUser)
                .ToDictionary(x => x.sku, y => y.userID);

            ConcurrentDictionary<string, string> shopifyUsersConcurrent =
                new ConcurrentDictionary<string, string>(shopifyUsers);

            ShopifyExcelCreator shopifyModifier =
                new ShopifyExcelCreator(profile, shopifyProfile, shopifyUsersConcurrent, path);

            try
            {
                shopifyModifier.ExcelGenerator();
            }
            catch (Exception e)
            {
                System.IO.File.Delete(path);
                return null;
            }

            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                 .AddEnvironmentVariables();

            IConfiguration Configuration;
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            string connectionstring = Configuration.GetConnectionString("BloggingDatabase");

            using (SqlConnection sourceConnection = new SqlConnection(connectionstring))
            {
                sourceConnection.Open();
                try
                {
                    DBModifierShopifyUserList user =
                        new DBModifierShopifyUserList(shopifyModifier.shopifyUser, profile);
                    user.TableExecutor();
                    // Execute raw query
                    
                    var command = sourceConnection.CreateCommand();
                    command.CommandText = "exec MergeUsersList;";
                    command.Connection = sourceConnection;
                    command.ExecuteNonQuery();
                    sourceConnection.Close();
                
                }
                catch (Exception e)
                {
                    sourceConnection.Close();
                    System.IO.File.Delete(path);
                    return null;
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(string file, string shipping
            , string fee, string profit, string promoting, string markdown, int items, int min
            , int max, string User)
        {
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), @"wwwroot\Excel_Source\Shopify_Upload.xlsx");

            Match shippingMatch = Regex.Match(shipping, @"[\d]+");

            Match amazonFee = Regex.Match(fee, @"[\d]+[/.]?[\d]+");

            Match promotingFee = Regex.Match(promoting, @"[\d]+[/.]?[\d]+");

            Match profitMatch = Regex.Match(profit, @"[\d]+");

            Profile oldProfile = _context.Profile.AsNoTracking().Where<Profile>(x => x.ProfileUser == User).FirstOrDefault();
            
            Profile profile = new Profile
            {
                shipping = Double.Parse(shippingMatch.Value),

                fee = Double.Parse(shippingMatch.Value),

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
            
            var shopifyProfile = _context.ShopifyUser.ToDictionary(x => x.sku, x => x);
            var fragrancex = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y);
            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y);
            var shopifyUsers = _context.UsersList.Where(x => x.userID == profile.ProfileUser)
                .ToDictionary(x => x.sku, y => y.userID);

            ConcurrentDictionary<string, string> shopifyUsersConcurrent = 
                new ConcurrentDictionary<string, string>(shopifyUsers);

            ShopifyExcelCreator shopifyModifier = 
                new ShopifyExcelCreator(profile, shopifyProfile, fragrancex, shopifyUsersConcurrent, upc, path);
            
            try
            {
                shopifyModifier.saveExcel();
            }
            catch (Exception e)
            {
                System.IO.File.Delete(path);
                return null;
            }

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

            return returnFile;
        }

        [HttpPost]
        public IActionResult Updatefragrancex()
        {
            /*
            ServiceTimeStamp service = new ServiceTimeStamp();

            DataTable uploadFragrancex = Helper.MakeFragrancexTable();

            var list = _context.Fragrancex.ToList();

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            long? value;
            
            int bulkSize = 0;
            
            Fragrancex fragrancex = new Fragrancex();

            //SDK Test and it works

            var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");

            var product = listingApiClient.GetProductById("412492");

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add(product.ItemId, product.ProductName);

            try
            {
                dic.Add(product.ItemId, product.ProductName);
                DataRow insideRow = uploadFragrancex.NewRow();

                insideRow["ItemID"] = Convert.ToInt32(product.ItemId);
                insideRow["BrandName"] = product.BrandName;
                insideRow["Description"] = product.Description;
                insideRow["Gender"] = product.Gender;
                insideRow["Instock"] = product.Instock;
                insideRow["LargeImageUrl"] = product.LargeImageUrl;
                insideRow["MetricSize"] = product.MetricSize;
                insideRow["ParentCode"] = product.ParentCode;
                insideRow["ProductName"] = product.ProductName;
                insideRow["RetailPriceUSD"] = product.RetailPriceUSD;
                insideRow["Size"] = product.Size;
                insideRow["SmallImageURL"] = product.SmallImageUrl;
                insideRow["Type"] = product.Type;
                insideRow["WholePriceAUD"] = product.WholesalePriceAUD;
                insideRow["WholePriceCAD"] = product.WholesalePriceCAD;
                insideRow["WholePriceEUR"] = product.WholesalePriceEUR;
                insideRow["WholePriceGBP"] = product.WholesalePriceGBP;
                insideRow["WholePriceUSD"] = product.WholesalePriceUSD;

                if (upc.TryGetValue(Convert.ToInt32(product.ItemId), out value))
                {
                    insideRow["Upc"] = value;
                }

                insideRow["UpcItemID"] = Convert.ToInt32(product.ItemId);

                uploadFragrancex.Rows.Add(insideRow);
                uploadFragrancex.AcceptChanges();
                bulkSize++;
            }
            catch
            {
                
            }
            
            Helper.upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");
            
            */

            updateFragrancex();

            return RedirectToAction("Update");
        }
        
        [HttpPost]
        public IActionResult UpdateFragrancexExcel(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            DBModifierFragrancexExcel dBModifierFragrancexExcel = new DBModifierFragrancexExcel(path, upc);

            dBModifierFragrancexExcel.TableExecutor();

            // Update the FragrancexList db

            var fragranceTitle = _context.FragrancexTitle.ToDictionary(x => x.ItemID, y => y.Title);

            DBModifierFragrancexExcelList dBModifierFragrancexExcelList = new DBModifierFragrancexExcelList(path, fragranceTitle);

            dBModifierFragrancexExcelList.TableExecutor();

            ServiceTimeStamp service = new ServiceTimeStamp();

            service.TimeStamp = DateTime.Today;

            service.Wholesalers = Wholesalers.Fragrancex.ToString();

            service.type = "Excel";

            _context.ServiceTimeStamp.Add(service);

            _context.SaveChanges();

            System.IO.File.Delete(path);

            return RedirectToAction("UpdateExcel");
        }

        private string test { get; set; }

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
            catch (Exception)
            {

            }
        }
    }
}