using System;
using System.Collections.Generic;

namespace CollectionApp.Application.Interfaces
{
    public interface IPagedResult<T>
    {
        IReadOnlyList<T> Items { get; }
        int TotalCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }
}

