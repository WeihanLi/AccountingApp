using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Models;

namespace AccountingApp.Helper
{
    public static class ExtensionHelper
    {
        public static PagedListModel<T> ToPagedListModel<T>(this IEnumerable<T> data,int pageIndex,int pageSize,int totalCount)
        {
            return new PagedListModel<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data.ToList()
            };
        }

        public static async Task<PagedListModel<T>> ToPagedListModelAsync<T>(this Task<IEnumerable<T>> data, int pageIndex, int pageSize, int totalCount)
        {
            var list = await data;
            return new PagedListModel<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = list.ToList()
            };
        }
    }
}
