using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SocialController : Controller
    {
        private readonly AppDbContext _context;
        public SocialController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Social> socials = await _context.Socials.Where(m => !m.IsDeleted).OrderByDescending(m => m.Id).ToListAsync();
            return View(socials);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Social social)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.Socials.AnyAsync(m => m.Name.Trim() == social.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Category already exist");
                    return View();
                }


                await _context.Socials.AddAsync(social);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }


        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Social social = await _context.Socials.FindAsync(id);

            if (social == null) return NotFound();

            return View(social);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Social social = await _context.Socials.FirstOrDefaultAsync(m => m.Id == id);

            social.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Social social = await _context.Socials.FirstOrDefaultAsync(m => m.Id == id);

                if (social == null) return NotFound();

                return View(social);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Social social)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                Social dbsocial = await _context.Socials.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbsocial == null) return NotFound();

                if (dbsocial.Name.Trim().ToLower() == social.Name.Trim().ToLower() && dbsocial.Url.Trim().ToLower() == social.Url.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }
              

                _context.Socials.Update(social);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
        }
    }
}
