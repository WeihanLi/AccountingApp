using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AccountingApp.Helper;

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

        [HttpGet]
        [ActionName("List")]
        public async Task<ActionResult> ListAsync(int pageIndex = 10, int pageSize = 20)
        {
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            int totalCount = await BusinessHelper.BillHelper.QueryCountAsync(b => !b.IsDeleted);
            var data = await BusinessHelper.BillHelper.SelectWithTypeInfoAsync(pageIndex, pageSize, b => !b.IsDeleted, b => b.CreatedTime);
            var list = data.ToPagedListModel(pageIndex, pageSize, totalCount);
            return Json(list, setting);
        }

        // GET: Bill/Create
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            ViewData["BillTypes"] = new ViewModels.BillTypeViewModel(await BusinessHelper.BillTypeHelper.SelectAsync(b => !b.IsDeleted, b => b.TypeName, true));
            return View();
        }

        // POST: Bill/Create
        [HttpPost,ActionName("Create")]
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
        [HttpGet,ActionName("Edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            ViewData["BillTypes"] = new ViewModels.BillTypeViewModel(await BusinessHelper.BillTypeHelper.SelectAsync(b => !b.IsDeleted, b => b.TypeName, true),id);
            return View(await BusinessHelper.BillHelper.FetchAsync(id));
        }

        // POST: Bill/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("BillTitle,BillDetails,BillType,BillFee,PKID")]Bill bill)
        {
            try
            {
                bill.UpdatedBy = User.Identity.Name;
                await BusinessHelper.BillHelper.UpdateAsync(bill, "BillTitle","BillDetails","BillType","BillFee", "UpdatedBy", "UpdatedTime");
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bill/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if(id<=0)
            {
                return NotFound();
            }
            var bill = await BusinessHelper.BillHelper.FetchAsync(id);
            if(bill == null)
            {
                return NotFound();
            }
            return View(bill);
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