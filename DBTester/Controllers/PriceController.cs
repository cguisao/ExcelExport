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

namespace DBTester.Controllers
{
    public class PriceController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Context _context;

        public PriceController(Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult PriceUpdater()
        {
            ViewBag.TimeStamp = _context.ServiceTimeStamp.LastOrDefault().TimeStamp.ToShortDateString();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductImport(IFormFile file)
        {
            return null;
        }
        }
}