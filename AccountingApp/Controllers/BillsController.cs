using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AccountingApp.Helper;
using MvcSimplePager;

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
        public async Task<ActionResult> ListAsync(int pageIndex = 1, int pageSize = 20)
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

        public ActionResult NewIndex()
        {
            return View();
        }

        /// <summary>
        /// 账单列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("BillsList")]
        public async Task<ActionResult> BillsListAsync(int pageIndex = 1, int pageSize = 10)
        {
            int totalCount = await BusinessHelper.BillHelper.QueryCountAsync(b => !b.IsDeleted);
            List<Bill> data = new List<Bill>();
            if (totalCount>0)
            {
                data = await BusinessHelper.BillHelper.SelectWithTypeInfoAsync(pageIndex, pageSize, b => !b.IsDeleted, b => b.CreatedTime);
            }
            return View(data.ToPagedList(pageIndex, pageSize, totalCount));
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

        /// <summary>
        /// 更新账单状态
        /// </summary>
        /// <param name="id">账单id</param>
        /// <param name="status">账单状态</param>
        /// <returns></returns>
        [HttpPost, ActionName("UpdateBillStatus")]
        public async Task<IActionResult> UpdateBillStatusAsync(int id, int status)
        {
            var result = new HelperModels.JsonResultModel();
            if (id <= 0 || status <= 0)
            {
                result.Status = HelperModels.JsonResultStatus.RequestError;
                result.Msg = "请求参数异常";
                return Json(result);
            }
            Bill bill = new Bill { PKID = id, BillStatus = status };
            bill.UpdatedBy = User.Identity.Name;
            await BusinessHelper.BillHelper.UpdateAsync(bill, b => b.BillStatus);
            result.Status = HelperModels.JsonResultStatus.Success;
            result.Msg = "操作成功";
            return Json(result);
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
                await BusinessHelper.BillHelper.DeleteAsync(m => m.PKID == id,User.Identity.Name);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}