using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DatabaseModifier;
using DBTester.Code;
using DBTester.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace GTI_Solutions.Controllers
{
    public class AzImporterController : Controller
    {
        public Context _context;

        public AzImporterController(Context context)
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
                .Where(x => x.Wholesalers == Wholesalers.AzImporter.ToString())
                .OrderByDescending(x => x.TimeStamp).Take(5).ToList());
        }

        public IActionResult Shipping()
        {
            return View(_context.Shipping);
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
        public IActionResult UpdateAzImportsExcel(string file)
        {
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            DBModifierAzImporterExcel AzImporter = new DBModifierAzImporterExcel(path);

            _context.Database.ExecuteSqlCommand("delete from AzImporter");

            AzImporter.TableExecutor();

            ServiceTimeStamp service = new ServiceTimeStamp();

            service.TimeStamp = DateTime.Today;

            service.Wholesalers = Wholesalers.AzImporter.ToString();

            service.type = "Excel";

            _context.ServiceTimeStamp.Add(service);

            _context.SaveChanges();

            System.IO.File.Delete(path);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public Task<IActionResult> UpdateAzImports()
        {
            //var client = new System.Net.WebClient();
            string url = @"http://www.azimporter.com/datafeed/dailydatafeed2.csv";
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "test.csv");

            //client.DownloadFile(url, path);

            var memory = new MemoryStream();

            using (var client = new WebClient())
            {
                var content = client.DownloadData(url);

                string csvDocument = Encoding.ASCII.GetString(content);

                var file = new FileInfo(path);

                using (FileStream fs = file.Create())
                {
                    //Add some information to the file.
                    fs.Write(content, 0, csvDocument.Length);
                }

                var lines = System.IO.File.ReadAllLines(path, Encoding.UTF8).Select(a => a.Split(','));

                List<AzImporter> items = new List<AzImporter>();
                int skipFirst = 1;

                foreach (var line in lines)
                {    
                    if (line.Length == 17)
                    {
                        int curr = 1;
                        AzImporter azImporter = new AzImporter();
                        foreach (var col in line)
                        {
                            // skip the first line because it is the title of the csv file
                            if (skipFirst == 1)
                                break;
                            // The first item is always the sku
                            if(curr == 1)
                            {
                                azImporter.Sku = col.Replace('"', ' ');
                            }
                            // loop until the first lines contains HTTP

                            // Once it does not have HTTP and it can be converted into a double
                            // we have the wholesale

                            // The next should the quantity and it can be turned into an integer

                            // Add the azImporter to the list
                            if (curr == 17)
                            {
                                items.Add(azImporter);
                            }

                            curr++;
                        }
                        skipFirst++;
                    }
                }

                //List<AzImporter> items = new List<AzImporter>();
                //int fieldCount = 0;
                //string[] headers;

                //using (TextFieldParser parser = new TextFieldParser(@"c:\temp\test.csv"))
                //{
                //    parser.TextFieldType = FieldType.Delimited;
                //    parser.SetDelimiters(",");
                //    while (!parser.EndOfData)
                //    {
                //        //Process row
                //        string[] fields = parser.ReadFields();
                //        foreach (string field in fields)
                //        {
                //            //TODO: Process field
                //        }
                //    }
                //}

                //using (CsvReader csv =  new CsvReader(file.OpenText()))
                //{
                //    fieldCount = csv.FieldCount;

                //    headers = csv.GetFieldHeaders();
                //    while (csv.ReadNextRecord())
                //    {
                //        for (int i = 0; i < fieldCount; i++)
                //            Console.Write(string.Format("{0} = {1};",
                //                          headers[i], csv[i]));
                //        Console.WriteLine();
                //    }
                //}

                using (var workbook = new XLWorkbook())
                {
                    //rowCount = 1;
                    AzImporter azImporter = new AzImporter();
                    //foreach (var line in csv.GetRecords<AzImporter>())
                    //{


                    //    colCount = 1;
                    //    //foreach (var col in line)
                    //    //{
                    //    //    if (rowCount != 1)
                    //    //    {
                    //    //        if(colCount == 1)
                    //    //        {
                    //    //            azImporter.Sku = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if(colCount == 2)
                    //    //        {
                    //    //            azImporter.Category = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 3)
                    //    //        {
                    //    //            azImporter.ItemName = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 4)
                    //    //        {
                    //    //            azImporter.Image1 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 5)
                    //    //        {
                    //    //            azImporter.Image2 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 6)
                    //    //        {
                    //    //            azImporter.Image3 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 7)
                    //    //        {
                    //    //            azImporter.Image4 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 8)
                    //    //        {
                    //    //            azImporter.Image5 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 9)
                    //    //        {
                    //    //            azImporter.Image6 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 10)
                    //    //        {
                    //    //            azImporter.Image7 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 11)
                    //    //        {
                    //    //            azImporter.Image8 = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 12)
                    //    //        {
                    //    //            azImporter.MainImage = col.Replace('"', ' ');
                    //    //        }
                    //    //        else if (colCount == 13)
                    //    //        {
                    //    //            azImporter.WholeSale = Convert.ToDouble(col.Replace('"', ' '));
                    //    //        }
                    //    //        else if (colCount == 14)
                    //    //        {
                    //    //            azImporter.Quantity = Convert.ToInt32(col.Replace('"', ' '));
                    //    //        }
                    //    //        else if (colCount == 15)
                    //    //        {
                    //    //            azImporter.Weight = Convert.ToInt32(col.Replace('"', ' '));
                    //    //        }
                    //    //        else if (colCount == 16)
                    //    //        {
                    //    //            azImporter.HTMLDescription = col.Replace('"', ' ');
                    //    //        }
                    //    //    }

                    //    //    colCount++;
                    //    //}
                    //    rowCount++;
                    //    items.Add(azImporter);
                    //}
                }
            }

            memory.Position = 0;


            //return new FileContentResult(bytes, MimeMapping.GetMimeMapping(f));

            return null;
        }

        [HttpPost]
        public IActionResult AzImportsShipping(int Weight, double Price)
        {
            Shipping shipping = new Shipping();
            shipping.ItemPrice = Price;
            shipping.weightId = Weight;

            if(_context.Shipping.Any(x => x.weightId == Weight))
            {
                _context.Shipping.Update(shipping);

                _context.SaveChanges();
            }
            else
            {
                _context.Shipping.Add(shipping);

                _context.SaveChanges();
            }

            return RedirectToAction("Shipping");
        }
    }
}