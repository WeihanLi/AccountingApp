using System;
using System.Collections;
using System.Collections.Generic;

namespace AccountingApp.HelperModels
{
    public class PagedListModel<T> 
    {
        private int pageIndex;

        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                if (value > 0)
                {
                    pageIndex = value;
                }
                else
                {
                    pageIndex = 1;
                }
            }
        }

        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (value > 0)
                {
                    pageSize = value;
                }
                else
                {
                    PageSize = 10;
                }
            }
        }

        private int totalCount;

        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                if (value > 0)
                {
                    totalCount = value;
                }
            }
        }

        public int PageCount { get { return Convert.ToInt32(Math.Ceiling(TotalCount * 1.0 / PageSize)); } }

        public List<T> Data { get; set; }
    }
}