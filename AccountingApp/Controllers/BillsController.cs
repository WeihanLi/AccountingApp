using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApp.Controllers
{
    public class BillsController : BaseController
    {
        public BillsController(AccountingDbContext context) : base(context)
        {
        }

        // GET: Bill
        public ActionResult Index()
        {
            return View();
        }

        // GET: Bill/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {
            return View(await BusinessHelper.BillHelper.FetchAsync(id));
        }

        // GET: Bill/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("BillTitle,BillDetails,BillType,BillFee")]Bill bill)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bill.CreatedBy = User.Identity.Name;
                    bill.UpdatedBy = User.Identity.Name;
                    await BusinessHelper.BillHelper.AddAsync(bill);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Bill/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Bill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("BillTitle,BillDetails,BillType,BillFee,PKID")]Bill bill)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bill/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(int id)
        {
            try
            {
                await BusinessHelper.BillHelper.DeleteAsync(m => m.PKID == id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}