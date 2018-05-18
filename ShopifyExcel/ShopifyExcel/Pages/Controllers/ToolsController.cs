using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ShopifyExcel.Pages.Code;
using EPPlus.Core;
using OfficeOpenXml;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ShopifyExcel.Pages.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ToolsController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public string UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "file not selected";

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }

            return Import(path);
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpGet]
        public ActionResult Index()
        {
            ToolsModel model = new ToolsModel();
            return View(model);
        }

        [HttpGet]
        public string Import(string sWebRootFolder)
        {
            //string sWebRootFolder = _hostingEnvironment.WebRootPath;
            //string sFileName = @"demo.xlsx";
            //FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            FileInfo file = new FileInfo(sWebRootFolder);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    bool bHeaderRow = true;
                    
                    for (int row = 1; row <= rowCount; row++)
                    {
                        for (int col = 1; col <= ColCount; col++)
                        {
                            if (col == 3 && row != 1)
                            {
                                worksheet.Cells[row, col].Value = Helper.BuildHTML(worksheet, row);
                            }
                            else if (col == 24 && row != 1)
                            {
                                worksheet.Cells[row, col].Value =
                                    worksheet.Cells[row, col].Value.ToString()
                                        .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                                        , "https://img.fragrancex.com/images/products/SKU/large/");
                            }
                            else if (col == 28 && row != 1)
                            {
                                worksheet.Cells[row, col].Value = Helper.UPCFinder();
                            }
                            if (bHeaderRow)
                            {
                                if(string.IsNullOrEmpty(sb.Append(worksheet.Cells[row, col].Value).ToString()))
                                {
                                    sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(sb.Append(worksheet.Cells[row, col].Value).ToString()))
                                {
                                    sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                                }
                            }
                        }
                        sb.Append(Environment.NewLine);
                    }
                    package.Save();
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Some error occurred while importing." + ex.Message;
            }
        }
        
        [HttpGet]
        public string ExportToExcel2()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
                //First add the headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Salary (in $)";

                //Add values
                worksheet.Cells["A2"].Value = 1000;
                worksheet.Cells["B2"].Value = "Jon";
                worksheet.Cells["C2"].Value = "M";
                worksheet.Cells["D2"].Value = 5000;

                worksheet.Cells["A3"].Value = 1001;
                worksheet.Cells["B3"].Value = "Graham";
                worksheet.Cells["C3"].Value = "M";
                worksheet.Cells["D3"].Value = 10000;

                worksheet.Cells["A4"].Value = 1002;
                worksheet.Cells["B4"].Value = "Jenny";
                worksheet.Cells["C4"].Value = "F";
                worksheet.Cells["D4"].Value = 5000;

                package.Save(); //Save the workbook.
            }
            return URL;
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(IFormFile file)
        {
            //List<Technology> technologies = StaticData.Technologies;
            //string[] columns = { "Name", "Project", "Developer" };
            //byte[] filecontent = ExcelExportHelper.ExportExcel(technologies, "Technology", true, columns);
            //return File(filecontent, ExcelExportHelper.ExcelContentType, "Technologies.xlsx");

            if (file == null || file.Length == 0)
                return null;

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string path2 = Import(path);

            //if (filename == null)
            //    return Content("filename not present");

            //var path = Path.Combine(
            //               Directory.GetCurrentDirectory(),
            //               "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));

            //return Download(filename);
        }
    }
}