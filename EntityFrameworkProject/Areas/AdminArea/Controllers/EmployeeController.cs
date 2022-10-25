using EntityFrameworkProject.Data;
using EntityFrameworkProject.Helpers;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, int take = 2)
        {
            List<Employee> employees = await _context.Employees
                .Where(m => !m.IsDeleted)
                .Skip((page * take) - take)
                .Take(take)
                .OrderByDescending(m => m.Id)
                .AsNoTracking()
                .ToListAsync();


            List<EmployeeListVM> mapDatas = GetMapDatas(employees);

            int count = await GetPageCount(take);

            Paginate<EmployeeListVM> result = new Paginate<EmployeeListVM>(mapDatas, page, count);

            return View(result);

        }

        private async Task<int> GetPageCount(int take)
        {
            int employeeCount = await _context.Employees.Where(m => !m.IsDeleted).CountAsync();
            return (int)Math.Ceiling((decimal)employeeCount / take);
        }


        private List<EmployeeListVM> GetMapDatas(List<Employee> employees)
        {
            List<EmployeeListVM> employeeList = new List<EmployeeListVM>();

            foreach (var employee in employees)
            {
                EmployeeListVM newEmployee = new EmployeeListVM
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    Age = employee.Age,
                    Position = employee.Position,
                    IsActive = employee.IsActive
                };
                employeeList.Add(newEmployee);
            }
            return employeeList;
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            Employee employee = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null) return NotFound();

            if (employee.IsActive)
            {
                employee.IsActive = false;
            }
            else
            {
                employee.IsActive = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }





        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                await _context.Employees.AddAsync(employee);

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

            Employee employee = await _context.Employees.FindAsync(id);

            if (employee == null) return NotFound();

            return View(employee);
        }







        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Employee employee = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);

                if (employee == null) return NotFound();

                return View(employee);
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
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View();
                }

                Employee dbemployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbemployee == null) return NotFound();

                if (dbemployee.FullName.Trim().ToLower() == employee.FullName.Trim().ToLower() && dbemployee.Age == employee.Age && dbemployee.Position.Trim().ToLower() == employee.Position.Trim().ToLower())
                {
                    return RedirectToAction(nameof(Index));
                }


                _context.Employees.Update(employee);

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



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Employee employee = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);

            employee.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }















}

