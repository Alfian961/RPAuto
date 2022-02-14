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
    public class InvTransactionController : Controller
    {
        private AppDbContext _dbContext = null;
        private List<InvTransaction> data = null;
        public InvTransactionController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet<InvTransaction> dbs = _dbContext.InvTransaction;
            data = dbs.ToList();
        }
        public IActionResult Index()
        {
            DbSet<InvTransaction> dbs = _dbContext.InvTransaction;
            List<InvTransaction> model = null;
            model = dbs.ToList();

            return View(model);
        }

        public IActionResult Create(int id)
        {
           
            return View();
        }
        public IActionResult SortDate()
        {
            var result =
               data.OrderBy(s => s.TransactionDate)
                   .ToList();

            ViewData["Title"] = "Sorting: TransactionDate";
            ViewData["Query"] = @"
var result =
               data.OrderBy(s => s.TransactionDate)
                   .ToList();
";

            return View("Index", result);
        }

        [HttpPost]
        public IActionResult Create(InvTransaction invtransaction)
        {
            if (ModelState.IsValid)
            {
                DbSet<InvTransaction> dbs = _dbContext.InvTransaction;
                dbs.Add(invtransaction);
                if (_dbContext.SaveChanges() == 1)
                {
                    TempData["Msg"] = "New transaction added!";
                    DbSet<Part> dbspart = _dbContext.Part;
                    Part part = dbspart.Where(m => m.PartNumber == invtransaction.PartNumber).FirstOrDefault();

                    if (part != null)
                    {
                        if (invtransaction.TransactionType == 0)
                        {
                            part.Qty = invtransaction.Qty + part.Qty;
                        }
                        else if (invtransaction.TransactionType == 1)
                        {
                            if ((part.Qty - invtransaction.Qty) > 0)
                            {
                                part.Qty = part.Qty - invtransaction.Qty;
                            }
                            else
                            {
                                TempData["Msg"] = "You can't create transactions that will reduce qty to less than 0!";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
        
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invtransaction);
        }

        public IActionResult PrintTransactions(String id) { 
        
            DbSet<InvTransaction> dbs = _dbContext.InvTransaction;
            List<InvTransaction> model = null;
            model = dbs.ToList();
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
