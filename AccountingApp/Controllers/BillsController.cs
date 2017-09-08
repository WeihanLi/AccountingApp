using AccountingApp.Helper;
using AccountingApp.Models;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using WeihanLi.AspNetMvc.MvcSimplePager;

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
            // 新版首页，新分页
            return View("NewIndex");
        }

        public async Task<ActionResult> ExportBillsReport()
        {
            var bills = await BusinessHelper.BillHelper.SelectWithTypeInfoAsync(b => !b.IsDeleted, b => b.CreatedTime);
            if (bills != null && bills.Any())
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("账单记录");
                // export excel
                int i = 0;
                var headRow = sheet.CreateRow(0);
                headRow.CreateCell(0, CellType.String).SetCellValue("账单标题");
                headRow.CreateCell(1, CellType.String).SetCellValue("账单类型");
                headRow.CreateCell(2, CellType.String).SetCellValue("账单金额");
                headRow.CreateCell(3, CellType.String).SetCellValue("账单详情");
                headRow.CreateCell(4, CellType.String).SetCellValue("创建人");
                headRow.CreateCell(5, CellType.String).SetCellValue("创建时间");

                foreach (var bill in bills)
                {
                    i++;
                    var row = sheet.CreateRow(i);
                    row.CreateCell(0, CellType.String).SetCellValue(bill.BillTitle);
                    row.CreateCell(1, CellType.String).SetCellValue(bill.AccountBillType.TypeName);
                    row.CreateCell(2, CellType.String).SetCellValue(bill.BillFee.ToString("0.00"));
                    row.CreateCell(3, CellType.String).SetCellValue(bill.BillDetails);
                    row.CreateCell(4, CellType.String).SetCellValue(bill.CreatedBy);
                    row.CreateCell(5, CellType.String).SetCellValue(bill.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                // 自动调整单元格的宽度
                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);
                sheet.AutoSizeColumn(2);
                sheet.AutoSizeColumn(3);
                sheet.AutoSizeColumn(4);
                sheet.AutoSizeColumn(5);
                var stream = new MemoryStream();
                workbook.Write(stream);
                return File(stream.ToArray(), "application/octet-stream","Bills.xls");
            }
            else
            {
                return Content("没有数据需要导出");
            }
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
            if (totalCount > 0)
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
        [HttpPost, ActionName("Create")]
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
        [HttpGet, ActionName("Edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            ViewData["BillTypes"] = new ViewModels.BillTypeViewModel(await BusinessHelper.BillTypeHelper.SelectAsync(b => !b.IsDeleted, b => b.TypeName, true), id);
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
                await BusinessHelper.BillHelper.UpdateAsync(bill, "BillTitle", "BillDetails", "BillType", "BillFee", "UpdatedBy", "UpdatedTime");
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
            if (id <= 0)
            {
                return NotFound();
            }
            var bill = await BusinessHelper.BillHelper.FetchAsync(id);
            if (bill == null)
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
                await BusinessHelper.BillHelper.DeleteAsync(m => m.PKID == id, User.Identity.Name);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}