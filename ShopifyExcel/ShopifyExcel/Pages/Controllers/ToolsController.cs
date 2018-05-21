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
            FileInfo file = new FileInfo(sWebRootFolder);
            Dictionary<string, long> dicSKU = new Dictionary<string, long>();
            Dictionary<string, string> dicTitle = new Dictionary<string, string>();

            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    dicSKU = Helper.UPCLoadDic();
                    dicTitle = Helper.titleDic(worksheet);
                    string SKU = "";

                    int count = 0;
                    string title = "";
                    for (int row = 1; row <= rowCount; row++)
                    {

                        if (row != 1)
                        {
                            title = Helper.BuildTitle(dicTitle, worksheet.Cells[row, 2].Value.ToString());
                            worksheet.Cells[row, 2].Value = title;
                            if (title.Length > 80)
                                count++;
                        }
                    }

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            worksheet.Cells[row, 3].Value = Helper.BuildHTML(worksheet, row, file.Name);
                        }
                    }

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            SKU = worksheet.Cells[row, 13].Value.ToString();
                        }
                    }

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (row != 1)
                        {
                            long value;
                            if (dicSKU.TryGetValue(SKU, out value))
                                if (dicSKU[SKU] != 0)
                                    worksheet.Cells[row, 23].Value = dicSKU[SKU];
                        }
                    }

                    for (int row = 1; row <= rowCount; row++)
                    {
                        worksheet.Cells[row, 24].Value =
                                       worksheet.Cells[row, 24].Value.ToString()
                                           .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                                           , "https://img.fragrancex.com/images/products/SKU/large/");
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

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(IFormFile file)
        {
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

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            
            return File(memory, GetContentType(path), Path.GetFileNameWithoutExtension(path) 
                + "_Converted" + Path.GetExtension(path).ToLowerInvariant());
        }
    }
}