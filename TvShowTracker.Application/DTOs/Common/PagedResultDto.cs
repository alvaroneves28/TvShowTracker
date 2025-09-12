using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs.Common
{
    /// <summary>
    /// Generic wrapper for paginated API responses containing a collection of items with pagination metadata.
    /// Provides comprehensive pagination information including navigation indicators and page calculations.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the paginated result</typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>
        /// The collection of items for the current page.
        /// Contains the actual data requested by the client within the specified page bounds.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// The current page number being returned, starting from 1.
        /// Indicates which page of results this response represents.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The number of items per page as requested by the client.
        /// Defines the maximum number of items that can appear in the Data collection.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items in the complete dataset across all pages.
        /// Represents the full count before pagination is applied.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The total number of pages available for the current dataset and page size.
        /// Calculated automatically based on TotalCount and PageSize.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates whether there are pages before the current page.
        /// True if CurrentPage is greater than 1, enabling previous page navigation.
        /// </summary>
        public bool HasPrevious { get; set; }

        /// <summary>
        /// Indicates whether there are pages after the current page.
        /// True if CurrentPage is less than TotalPages, enabling next page navigation.
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// Initializes a new instance of PagedResultDto with the provided data and pagination parameters.
        /// Automatically calculates derived pagination properties for client convenience.
        /// </summary>
        /// <param name="data">The collection of items for the current page</param>
        /// <param name="currentPage">The current page number (1-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <param name="totalCount">The total number of items in the complete dataset</param>
        public PagedResultDto(IEnumerable<T> data, int currentPage, int pageSize, int totalCount)
        {
            Data = data;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;

            // Calculate total pages using ceiling division to include all items
            // This ensures that even partial last pages are counted
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Calculate navigation flags based on current position
            HasPrevious = currentPage > 1;
            HasNext = currentPage < TotalPages;
        }
    }

    /// <summary>
    /// Data transfer object containing query parameters for filtering, sorting, and paginating API requests.
    /// Provides a standardized way to specify search criteria across different endpoints.
    /// </summary>
    public class QueryParameters
    {
        /// <summary>
        /// The page number to retrieve, starting from 1.
        /// Used to calculate which subset of results to return.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// The number of items to return per page.
        /// Controls the size of each page in paginated results.
        /// </summary>
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// The field name to sort results by.
        /// Should correspond to a sortable property of the target entity.
        /// </summary>
        public string SortBy { get; set; } = "Name";

        /// <summary>
        /// Indicates whether to sort in descending order.
        /// False (default) sorts in ascending order, true sorts in descending order.
        /// </summary>
        public bool SortDescending { get; set; } = false;

        /// <summary>
        /// Optional filter to return only items of a specific genre.
        /// Case-insensitive matching against genre classifications.
        /// </summary>
        public string? Genre { get; set; }

        /// <summary>
        /// Optional filter to return only items of a specific show type.
        /// Filters by the format/category of the TV content.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Optional search query for full-text search across relevant fields.
        /// Searches show names, descriptions, and other searchable content.
        /// </summary>
        [StringLength(100, ErrorMessage = "Search query cannot exceed 100 characters")]
        public string? Search { get; set; }
    }
}