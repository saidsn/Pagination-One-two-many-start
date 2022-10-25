using EntityFrameworkProject.Data;
using EntityFrameworkProject.Helpers;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        } 
        public async Task<IActionResult> Index(int page = 1,int take = 2)
        {
            List<Category> categories = await _context.Categories
                .Where(m=>!m.IsDeleted)
                .Skip((page*take)-take)
                .Take(take)
                .ToListAsync();

            List<CategoryListVM> mapDatas = GetMapDatas(categories);

            int count = await GetPageCount(take);

            Paginate<CategoryListVM> result = new Paginate<CategoryListVM>(mapDatas, page, count);

            return View(result);

        }

        private async Task<int> GetPageCount(int take)
        {
            int categoryCount = await _context.Categories.Where(m => !m.IsDeleted).CountAsync();
            return (int)Math.Ceiling((decimal)categoryCount / take);
        }


        private  List<CategoryListVM> GetMapDatas(List<Category> categories)
        {
            List<CategoryListVM> categoryList = new List<CategoryListVM>();

            foreach (var category in categories)
            {
                CategoryListVM newCategory = new CategoryListVM
                {
                    Id = category.Id,
                    Name = category.Name
                };
                categoryList.Add(newCategory);
            }
            return categoryList;
        }

















        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.Categories.AnyAsync(m => m.Name.Trim() == category.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Category already exist");
                    return View();
                }


                //change exist data is delete

                //var data = await _context.Categories.Where(m => m.IsDeleted == true).FirstOrDefaultAsync(m => m.Name.Trim() == category.Name.Trim());

                //if (data == null)
                //{
                //    await _context.Categories.AddAsync(category);

                //}
                //else
                //{
                //    data.IsDeleted = false;
                //}
                await _context.Categories.AddAsync(category);

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

            Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            return View(category);
        }


        // Soft delete from database


        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

        //    _context.Categories.Remove(category);

        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

                if (category == null) return NotFound();

                return View(category);
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
        public async Task<IActionResult> Edit(int id, Category category)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                Category dbcategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbcategory == null) return NotFound();

                if (dbcategory.Name.Trim().ToLower() == category.Name.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }
                //dbcategory.Name = category.Name;

                _context.Categories.Update(category);

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
