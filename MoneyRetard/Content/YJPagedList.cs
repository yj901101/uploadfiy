using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyRetard.Content
{
    public class YJPagedList<T> : List<T>
    {
        public int? PageIndex { get; private set; }//当前页号减一的值 
        public int PageSize { get; private set; }　//每页显示的内容数量 
        public int TotalPages { get; private set; }//总页数 
        public int Start { get; private set; }//当前页面，显示的第一个页号（比如在中间的页面，页号显示是9号到16号，9就是Start） 
        public int End { get; private set; }//当前页面，显示的最后一个页号 

        public YJPagedList(IQueryable<T> source, int? pageIndex, int pageSize)
        {
            PageIndex = (pageIndex ?? 0);
            PageSize = pageSize; ;
            TotalPages = (int)Math.Ceiling(source.Count() / (double)PageSize);

            int size;//判定每个页面显示多少个页号 
            if (TotalPages > 6)//这里规定每个页面显示6个页号 
            {
                size = 6;
                //定义每个页面的页号从几开始 
                if (pageIndex > 2 && pageIndex < TotalPages - (size - 2))
                {
                    Start = (pageIndex ?? 0) - 1;
                }
                else if (pageIndex >= TotalPages - (size - 2))
                {
                    Start = TotalPages - size + 1;
                }
                else
                {
                    Start = 1;
                }
            }
            else
            {
                size = TotalPages;
                Start = 1;

            }

            End = Start + size - 1;
            //将实体对象分页后传入 
            this.AddRange(source.Skip(PageSize * (PageIndex ?? 0)).Take(PageSize));
        }

        // 为“上一页”“下一页”导航备用 
        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }

    }
}