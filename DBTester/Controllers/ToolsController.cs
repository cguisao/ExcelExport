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
using X.PagedList;
using System.Text;
using OfficeOpenXml;
using FrgxPublicApiSDK;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using FastMember;
using System.Data;
using FrgxPublicApiSDK.Models;

namespace DBTester.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;

        public Context _context2;
 
        public ToolsController(Context context, Context context2, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _context2 = context2;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Tools()
        {
            return View();
        }

        public IActionResult Upcs()
        {
            return View();
        }

        public IActionResult Update()
        {
            return View(_context.ServiceTimeStamp.ToList());
        }
        
        public IActionResult ProductDownload()
        {
            return View();
        }

        public IActionResult UpcViewer(int? page, string Search_Data)
        {
            var upcs = _context.UPC.ToList();

            var pageNumber = page ?? 1;

            var onePageOfUpcs = upcs.ToPagedList(pageNumber, 10);
            
            ViewBag.onePageOfUpcs = onePageOfUpcs;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UPCImporter(IFormFile file)
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
            Helper.UPCLoadDic(path);
            
            return RedirectToAction("UpcViewer");
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
                Helper.UPCTester(path);

                service.TimeStamp = DateTime.Today;
                _context.ServiceTimeStamp.Add(service);
                _context.SaveChanges();

            }
            else if(_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
            {
                Helper.UPCTester(path);

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
        public async Task<IActionResult> ExportToExcel(IFormFile file)
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

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            var price = _context.Fragrancex.ToDictionary(x => x.ItemID, y => y.WholePriceUSD);
            
            Helper.ExcelGenerator(path, price, upc);
            
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
        public async Task<IActionResult> Updatefragrancex()
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

            var product = listingApiClient.GetProductById("482296");

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

            Helper.upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");
            
            */

            ServiceTimeStamp service = new ServiceTimeStamp();

            DataTable uploadFragrancex = Helper.MakeFragrancexTable();

            var list = _context.Fragrancex.ToList();

            var upc = _context.UPC.ToDictionary(x => x.ItemID, y => y.Upc);

            long? value;

            int bulkSize = 0;

            if (_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>() == null)
            {
                var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");
                

                Fragrancex fragrancex = new Fragrancex();
                
                var allProducts = listingApiClient.GetAllProducts();
                
                foreach (var product in allProducts)
                {
                    if (product != null)
                    {
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
                }
                
                Helper.upload(uploadFragrancex, bulkSize, "dbo.Fragrancex");

                service.TimeStamp = DateTime.Today;
                _context.ServiceTimeStamp.Add(service);
                _context.SaveChanges();
            }
            else if (_context.ServiceTimeStamp.LastOrDefault<ServiceTimeStamp>().TimeStamp != DateTime.Today)
            {
                //SDK Test and it works 
                //var listingApiClient = new FrgxListingApiClient("346c055aaefd", "a5574c546cbbc9c10509e3c277dd7c7039b24324");

                //var allProducts = listingApiClient.GetAllProducts();

                //foreach (var product in allProducts)
                //{
                //    if (product != null)
                //    {
                //        Fragrancex fragrancex = NewMethod(product);

                //        try
                //        {
                //            _context.Fragrancex.Add(fragrancex);
                //            _context.SaveChanges();
                //        }
                //        catch (Exception e)
                //        {
                //            _context.Fragrancex.Update(fragrancex);
                //            await _context.SaveChangesAsync();
                //            continue;
                //        }
                //    }
                //}
                //service.TimeStamp = DateTime.Today;
                //_context.ServiceTimeStamp.Add(service);
                //_context.SaveChanges();
            }
            
            return RedirectToAction("Update");
        }
    }
}