using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagementSystem.ApplicationCore.Entities
{
    public abstract class PagingOptionBase
    {
        public int CurrentPage { get; set; } 
        public int PageCount { get; set; } 
        public int SizePerPage { get; set; } 
        public int RowCountTotal { get; set; }
        public int SkipValue { get; set; }
    }

    public class PagingOption<T> : PagingOptionBase where T : class
    {
        public IList<T> ResultList { get; set; } = new List<T>();

        // [JsonIgnore] public bool SkipPaging { get; set; } = false;

        public PagingOption(int currentPage, int sizePerPage)
        {
            CurrentPage = currentPage;
            SizePerPage = sizePerPage;

            SkipValue = (CurrentPage - 1) * SizePerPage;
        }
        public void ExecuteResourcePaging() 
        {
            RowCountTotal = ResultList.Count;
            if (CurrentPage == 0 && SizePerPage == 0)
                PageCount = 1;
            else
                PageCount = (int) Math.Ceiling((double)RowCountTotal / SizePerPage);
            
            if (!(CurrentPage == 0 && SizePerPage == 0))
                ResultList = ResultList.Skip(SkipValue).Take(SizePerPage).ToList();
        }
        
        public void ExecuteResourcePaging(int overideRowCountTotalNumber)
        {
            RowCountTotal = overideRowCountTotalNumber;
            if (CurrentPage == 0 && SizePerPage == 0)
                PageCount = 1;
            else
                PageCount = (int) Math.Ceiling((double)RowCountTotal / SizePerPage);
            
            if (!(CurrentPage == 0 && SizePerPage == 0))
                ResultList = ResultList.Skip(SkipValue).Take(SizePerPage).ToList();
        }
    }
}