using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace GTI_Solutions.Controllers
{
    public class ProfileController : Controller
    {
        public Context _context;

        public ProfileController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile(int? page)
        {
            var profiles = _context.Profile.ToList();

            var pageNumber = page ?? 1;

            var onePageOfProfiles = profiles.ToPagedList(pageNumber, 10);

            ViewBag.onePageOfProfiles = onePageOfProfiles;

            return View();
        }

        [HttpPost]
        public IActionResult ProfileCreator(string userID, string html)
        {
            Profile profile = new Profile();

            profile.ProfileUser = userID;

            profile.html = html;

            _context.Profile.Add(profile);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}