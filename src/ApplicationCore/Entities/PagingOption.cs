using System;
using System.Collections.Generic;

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
        public IList<T> Results { get; set; }
        public bool NoPaging { get; set; }
        public PagingOption(int currentPage, int sizePerPage)
        {
            CurrentPage = currentPage;
            SizePerPage = sizePerPage;
            PageCount = (int) Math.Ceiling((double)RowCountTotal / SizePerPage);
            SkipValue = (CurrentPage - 1) * SizePerPage;
            
            if (CurrentPage == 0 && SizePerPage==0)
                NoPaging = true;
        }
    }
}