using System.Collections.Generic;

namespace Tracker_Money.Models
{
	public interface IPagedViewModel
	{
		int PageNumber { get; }
		int PageSize { get; }
		int TotalCount { get; }
		int TotalPages { get; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }
		string? SearchTerm { get; }
		string? OrderBy { get; }
		IDictionary<string, object?> RouteValues { get; }
	}
}
