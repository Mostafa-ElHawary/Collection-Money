using CollectionApp.Application.Interfaces;
using System.Collections.Generic;

namespace Tracker_Money.Models
{
    public class PagedViewModel<T> : IPagedViewModel
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
        public IDictionary<string, object?> RouteValues { get; set; } = new Dictionary<string, object?>();

        public PagedViewModel()
        {
        }
        
        public PagedViewModel(IPagedResult<T> pagedResult)
        {
            Items = pagedResult.Items;
            PageNumber = pagedResult.PageNumber;
            PageSize = pagedResult.PageSize;
            TotalCount = pagedResult.TotalCount;
            TotalPages = pagedResult.TotalPages;
            HasPreviousPage = pagedResult.HasPreviousPage;
            HasNextPage = pagedResult.HasNextPage;
        }
    }
}