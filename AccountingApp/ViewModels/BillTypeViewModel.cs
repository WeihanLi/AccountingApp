using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApp.ViewModels
{
    public class BillTypeViewModel
    {
        public List<SelectListItem> BillTypes;

        public BillTypeViewModel()
        { }

        public BillTypeViewModel(IEnumerable<Models.BillType> types)
        {
            BillTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "-1", Text = "请选择", Selected = true }
            };
            BillTypes.AddRange(types.Select(t => new SelectListItem { Value = t.PKID.ToString(), Text = t.TypeName }));
        }

        public BillTypeViewModel(IEnumerable<Models.BillType> types,int id)
        {
            BillTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "-1", Text = "请选择" }
            };
            BillTypes.AddRange(types.Select(t => new SelectListItem { Value = t.PKID.ToString(), Text = t.TypeName, Selected = (t.PKID == id) }));

        }
    }
}
