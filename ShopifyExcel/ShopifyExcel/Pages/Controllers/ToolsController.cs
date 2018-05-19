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
            Dictionary<string, long> dic = new Dictionary<string, long>();
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    bool bHeaderRow = true;
                    dic = Helper.UPCLoadDic();
                    string SKU = "";
                    for (int row = 1; row <= rowCount; row++)
                    {
                        for (int col = 1; col <= ColCount; col++)
                        {
                            
                            if (col == 3 && row != 1)
                            {
                                worksheet.Cells[row, col].Value = Helper.BuildHTML(worksheet, row, file.Name);
                            }
                            else if (col == 13 && row != 1)
                            {
                                SKU = worksheet.Cells[row, col].Value.ToString();
                            }
                            else if (col == 23 && row != 1)
                            {
                                long value;
                                if(dic.TryGetValue(SKU, out value))
                                    if (dic[SKU] != 0)
                                        worksheet.Cells[row, col].Value = dic[SKU];
                            }
                            else if (col == 24 && row != 1)
                            {
                                worksheet.Cells[row, col].Value =
                                    worksheet.Cells[row, col].Value.ToString()
                                        .Replace("http://img.fragrancex.com/images/products/SKU/small/"
                                        , "https://img.fragrancex.com/images/products/SKU/large/");
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
                + "Converted" + Path.GetExtension(path).ToLowerInvariant());
        }
    }
}