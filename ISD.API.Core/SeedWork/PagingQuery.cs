using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.API.Core.SeedWork
{
    public class PagingQuery
    {
        /// <summary>
        /// Init
        /// </summary>
        public PagingQuery()
        {
            PageIndex = 1;
            PageSize = 20;
            OrderBy = string.Empty;
            OrderByDesc = string.Empty;
        }

        /// <summary>
        /// Search
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Limit
        /// </summary>        
        public int PageSize { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page Index chỉ bao gồm giá trị số lớn hơn 0")]
        public int PageIndex { get; set; }

        public int Offset
        {
            get
            {
                return PageIndex * PageSize - PageSize;
            }
        }
        public int Limit
        {
            get
            {
                return PageSize;
            }
        }

        /// <summary>
        /// Order by asc/desc
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Has order by desc
        /// </summary>
        public string OrderByDesc { get; set; }
    }
    public class PagingITP
    {
        public PagingQuery PagingQuery { get; set; } = new PagingQuery();      
    }
}
