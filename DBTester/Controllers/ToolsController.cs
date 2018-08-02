﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using DBTester.Code;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
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

namespace DBTester.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;
 
        public ToolsController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Tools()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp.LastOrDefault().TimeStamp.ToShortDateString();

            ViewBag.type = _context.ServiceTimeStamp.LastOrDefault().type;

            var profile = new Profile();

            return View(_context.Profile.ToList());
        }
        
        public IActionResult Update()
        {
            return View(_context.ServiceTimeStamp.OrderByDescending(x => x.TimeStamp).ToList());
        }
        
        public IActionResult UpdateExcel()
        {
            return View(_context.ServiceTimeStamp.OrderByDescending(x => x.TimeStamp).ToList());
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
                //newFile.CopyTo(stream.ToString());
                await file.CopyToAsync(stream);
            }

            ServiceTimeStamp service = new ServiceTimeStamp();

            if (_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>() == null)
            {
                Helper.tablePreparer(path);

                service.TimeStamp = DateTime.Today;
                _context.ServiceTimeStamp.Add(service);
                _context.SaveChanges();

            }
            else if(_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
            {
                Helper.tablePreparer(path);

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
        public async Task<IActionResult> ExportToExcel(IFormFile file, string shipping
            , string fee, string profit, string promoting, string markdown, int items, int min, int max, string User)
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

            IExcelExtension shopifyModifier = new ShopifyExcelCreator(upc, profile)
            {
                sWebRootFolder = path,
                prices = prices
            };

            shopifyModifier.ExcelGenerator();
            
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
        public async Task<IActionResult> UpdateFragrancexExcel(IFormFile file)
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

            // Update the DB with the new UPCs

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            _context.Database.ExecuteSqlCommand("delete from Fragrancex");

            DBModifierFragrancexExcel dBModifierFragrancexExcel = new DBModifierFragrancexExcel(path, upc);

            dBModifierFragrancexExcel.TableExecutor();

            ServiceTimeStamp service = new ServiceTimeStamp();

            service.TimeStamp = DateTime.Today;

            service.type = "Excel";

            _context.ServiceTimeStamp.Add(service);

            _context.SaveChanges();

            return RedirectToAction("UpdateExcel");
        }

        private void updateFragrancex()
        {
            ServiceTimeStamp service = new ServiceTimeStamp();

            if (_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>() == null)
            {
                FragancexSQLPreparer(service);
            }
            else if (_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
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

                _context.Database.ExecuteSqlCommand("delete from Fragrancex");

                DBModifierFragrancexAPI dBModifierFragrancexAPI = new DBModifierFragrancexAPI("", upc)
                {
                    allProducts = allProducts
                };

                dBModifierFragrancexAPI.TableExecutor();

                service.TimeStamp = DateTime.Today;

                service.type = "API";

                _context.ServiceTimeStamp.Add(service);

                _context.SaveChanges();
            }
            catch(Exception e)
            {

            }
        }
    }
}