using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using RPAuto.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;

namespace RPAuto.Controllers
{
    public class PartsController : Controller
    {
        private AppDbContext _dbContext = null;
        private List<Part> data = null;

        public PartsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet<Part> dbs = _dbContext.Part;
            data = dbs.ToList();
        }

        public IActionResult Index()
        {
            DbSet<Part> dbs = _dbContext.Part;
            List<Part> model = null;
            model = dbs.Include(co => co.Category)
                        .Include(co => co.InvTransaction)
                        .ToList();

            return View(model);
        }

        public IActionResult PartView()
        {
            DbSet<Part> dbs = _dbContext.Part;
            List<Part> model = null;
            model = dbs.Include(co => co.Category)
                        .Include(co => co.InvTransaction)
                        .ToList();

            // TODO Lesson08 Ex1a Return the partial view _Ex1.
            // Navigate to Home/PPULoadPageEx1 to test the partial view
            return PartialView("_PartPart", model);
        }

        public IActionResult SortCatid()
        {
            var result =
               data.OrderBy(s => s.CategoryId)
                   .ThenBy(s => s.Category)
                   .ThenByDescending(s => s.Title)
                   .ToList();

            ViewData["Title"] = "Sorting: CategoryId, Category, Title(desc)";
            ViewData["Query"] = @"
var result =
               data.OrderBy(s => s.CategoryId)
                   .ThenBy(s => s.Category)
                   .ThenByDescending(s => s.Title)
                   .ToList();
";

            return View("Index", result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Part parts)
        {
            if (ModelState.IsValid)
            {
                Part insert = new Part
                {
                    PartNumber = parts.PartNumber,
                    Title = parts.Title,
                    Overview = parts.Overview,
                    CategoryId = parts.CategoryId,
                    Qty = 0
                };


                _dbContext.Part.Add(insert);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(parts);
        }
        
        public IActionResult Update(String id)
        {
            DbSet<Part> dbs = _dbContext.Part;
            Part part = dbs.Where(mo => mo.PartNumber.Equals(id)).FirstOrDefault();

            if (part != null)
            {
                DbSet<Part> dbsPart = _dbContext.Part;
                var lstPart = dbsPart.ToList();
                ViewData["parts"] = new SelectList(lstPart, "PartNumber", "Title");

                return View(part);
            }
            else
            {
                TempData["Msg"] = "Part not found!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Update(Part part)
        {
            if (ModelState.IsValid)
            {
                DbSet<Part> dbs = _dbContext.Part;

                Part newpart = dbs.Where(m => m.PartNumber.Equals(part.PartNumber)).FirstOrDefault();

                if (newpart != null)
                {
                    newpart.PartNumber = part.PartNumber;
                    newpart.Title = part.Title;
                    newpart.Overview= part.Overview;
                    newpart.CategoryId = part.CategoryId;
                    newpart.Qty = part.Qty;

                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Parts updated!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Part not found!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Msg"] = "Invalid information entered";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(String id)
        {
            DbSet<Part> dbs = _dbContext.Part;

            Part part = dbs.Where(m => m.PartNumber.Equals(id)).FirstOrDefault(); 

            if (part != null)
            {
                if (part.Qty <1) { 

                    dbs.Remove(part);
                    if (_dbContext.SaveChanges() == 1)
                        TempData["Msg"] = "Part deleted!";
                    else
                        TempData["Msg"] = "Failed to update database!";
                }
                else
                {
                    TempData["Msg"] = "Can't delete if you have 1 or more in stock!";
                }
            }
            else
            {
                TempData["Msg"] = "Part not found!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult PrintPart(String id)
        {
            DbSet<Part> dbs = _dbContext.Part;
            List<Part> model = dbs.Include(co => co.Category)
                        .Include(co => co.InvTransaction)
                        .ToList();
            if (model != null)
                return new ViewAsPdf(model)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.B5,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
                };
            else
            {
                TempData["Msg"] = "Parts not found3!";
                return RedirectToAction("Index");
            }

        }
    }
}
