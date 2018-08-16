using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBTester.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace GTI_Solutions.Controllers
{
    public class ProfileController : Controller
    {
        public Context _context;
        public List<SelectListItem> items = new List<SelectListItem>();

        public ProfileController(Context context)
        {
            _context = context;

            items.Add(new SelectListItem
            {
                Text = "--Select--",
                Value = "0",
                Selected = true
            });

            items.Add(new SelectListItem
            {
                Text = "For Women/Men",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Perfume/Cologne",
                Value = "2"
            });

            items.Add(new SelectListItem
            {
                Text = "None",
                Value = "3"
            });
        }

        public IActionResult Index()
        {
            ViewBag.CategoryType = items;

            return View(_context.Profile.ToList());
        }

        public IActionResult Title()
        {
            ViewBag.CategoryType = items;

            return View(_context.Profile.ToList());
        }

        public IActionResult Profile(int? page)
        {
            var profiles = _context.Profile.ToList();

            var pageNumber = page ?? 1;

            //var onePageOfProfiles = profiles.ToPagedList(pageNumber, 10);

            //ViewBag.onePageOfProfiles = onePageOfProfiles;

            return View(_context.Profile.ToList());
        }

        [HttpPost]
        public IActionResult ProfileCreator(string userID, string html, string longTitle
            , string MidTitle, string shortTitle, string sizeDivider, string endTitle)
        {
            Profile profile = new Profile
            {
                ProfileUser = userID,

                html = html,

                LongstartTitle = longTitle,

                MidtartTitle = MidTitle,

                ShortstartTitle = shortTitle,

                sizeDivider = sizeDivider,

                endTtile = items.Where(x => x.Value == endTitle).Select(x => x.Text).FirstOrDefault()
            };


            _context.Profile.Add(profile);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ProfileUpdator(string User, string html, string longTitle
            , string MidTitle, string shortTitle, string sizeDivider, string endTitle)
        {
            Profile profile = new Profile();

            Profile oldProfile = _context.Profile.AsNoTracking().Where<Profile>(x => x.ProfileUser == User).FirstOrDefault();

            profile.ProfileUser = User;

            profile.html = oldProfile.html;

            profile.items = oldProfile.items;

            profile.markdown = oldProfile.markdown;

            profile.max = oldProfile.max;

            profile.min = oldProfile.min;

            profile.profit = oldProfile.profit;

            profile.promoting = oldProfile.promoting;

            profile.shipping = oldProfile.shipping;

            profile.LongstartTitle = longTitle;

            profile.MidtartTitle = MidTitle;

            profile.ShortstartTitle = shortTitle;

            profile.sizeDivider = sizeDivider;

            profile.endTtile = items.Where(x => x.Value == endTitle).Select(x => x.Text).FirstOrDefault();

            _context.Profile.Update(profile);

            _context.SaveChanges();

            return RedirectToAction("Title");
        }
    }
}