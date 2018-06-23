using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using Microsoft.AspNetCore.Mvc;

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