using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AccountingApp.Models;
using Microsoft.Extensions.Logging;

namespace AccountingApp.Controllers
{
    public class BillTypesController : BaseController
    {
        public BillTypesController(AccountingDbContext context, ILogger<BillTypesController> logger) : base(context, logger)
        {
        }

        // GET: BillTypes
        public async Task<IActionResult> Index()
        {
            return View(await BusinessHelper.BillTypeHelper.SelectAsync(t=>true,t=>t.CreatedTime));
        }

        // GET: BillTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var billType = await BusinessHelper.BillTypeHelper.FetchAsync(m => m.PKID == id);
            if (billType == null)
            {
                return NotFound();
            }
            return View(billType);
        }

        // GET: BillTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BillTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeName,TypeDesc")] BillType billType)
        {
            if (ModelState.IsValid)
            {
                billType.CreatedBy = User.Identity.Name;
                await BusinessHelper.BillTypeHelper.AddAsync(billType);
                return RedirectToAction("Index");
            }
            return View(billType);
        }

        // GET: BillTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billType = await BusinessHelper.BillTypeHelper.FetchAsync(m => m.PKID == id);
            if (billType == null)
            {
                return NotFound();
            }
            return View(billType);
        }

        // POST: BillTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeName,TypeDesc,PKID")] BillType billType)
        {
            if (id != billType.PKID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    billType.UpdatedBy = User.Identity.Name;
                    await BusinessHelper.BillTypeHelper.UpdateAsync(billType,"TypeName","TypeDesc","UpdatedBy","UpdatedTime");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BusinessHelper.BillTypeHelper.ExistAsync(e => e.PKID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(billType);
        }

        // GET: BillTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billType = await BusinessHelper.BillTypeHelper.FetchAsync(id.Value);
            if (billType == null)
            {
                return NotFound();
            }

            return View(billType);
        }

        // POST: BillTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await BusinessHelper.BillTypeHelper.DeleteAsync(t => t.PKID == id,User.Identity.Name);
            return RedirectToAction("Index");
        }
    }
}
