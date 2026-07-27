using FinTrack_II_Trimestre.Data;
using FinTrack_II_Trimestre.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack_II_Trimestre.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Profile
        public IActionResult Index()
        {
            IEnumerable<Profile> profiles = _db.Profiles;
            return View(profiles);
        }

        // GET: Profile/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Profile profile)
        {
            if (ModelState.IsValid)
            {
                _db.Profiles.Add(profile);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Profile/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            var profile = _db.Profiles.Find(id);
            if (profile == null) { return NotFound(); }
            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Profile profile)
        {
            if (ModelState.IsValid)
            {
                _db.Profiles.Update(profile);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Profile/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            var profile = _db.Profiles.FirstOrDefault(p => p.ProfileId == id);
            if (profile == null) { return NotFound(); }
            return View(profile);
        }

        // POST: Profile/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            var profile = _db.Profiles.Find(id);
            if (profile == null) { return NotFound(); }
            _db.Profiles.Remove(profile);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
