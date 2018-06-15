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
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace GTI_Solutions.Controllers
{
    public class UpcController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;

        public UpcController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Upcs()
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
            UpcHelper.UPCLoadDic(path);

            return RedirectToAction("UpcViewer");
        }
    }
}