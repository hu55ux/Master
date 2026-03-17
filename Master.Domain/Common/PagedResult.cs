namespace Master.Application.Common
{
    /// <summary>
    /// Represents a paged result of a collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The size of each page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The total number of pages based on PageSize and TotalCount.
        /// </summary>
        public int TotalPages
            => Convert.ToInt32(Math.Ceiling(TotalCount / (double)PageSize));

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPrevious
            => Page > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNext
            => Page < TotalPages;

        /// <summary>
        /// Creates a new instance of <see cref="PagedResult{T}"/>.
        /// </summary>
        /// <param name="items">The items for the current page.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total number of items.</param>
        /// <returns>A new <see cref="PagedResult{T}"/> instance.</returns>
        public static PagedResult<T> Create(
            IEnumerable<T> items,
            int page,
            int pageSize,
            int totalCount)
        {
            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}