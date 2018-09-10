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
using DatabaseModifier;

namespace DBTester.Controllers
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
            Guid guid = Guid.NewGuid();

            ViewBag.ExcelGuid = guid.ToString();

            return View(_context.UPC.ToList());
        }

        public IActionResult UpcViewer(int? page, string Search_Data)
        {
            var upcs = _context.UPC.ToList();

            var pageNumber = page ?? 1;

            //var onePageOfUpcs = upcs.ToPagedList(pageNumber, 10);

            //ViewBag.onePageOfUpcs = onePageOfUpcs;

            return View();
        }

        [HttpPost]
        public IActionResult UPCImporter(string file)
        {

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        file + ".xlsx");

            // Update the DB with the new UPCs

            DBModifierUPC databaseUPC = new DBModifierUPC(path);

            databaseUPC.TableExecutor();

            System.IO.File.Delete(path);

            return RedirectToAction("Upcs");
        }
    }
}