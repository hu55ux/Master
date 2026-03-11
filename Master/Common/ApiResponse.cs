namespace Master.Common
{
    /// <summary>
    /// Represents a standardized API response.
    /// Can be used for both success and error responses.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// A human-readable message describing the result of the operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The payload data returned from the API.
        /// Can be null if there is no data or in case of an error.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful API response with optional data and message.
        /// </summary>
        /// <param name="data">The payload data to return.</param>
        /// <param name="message">Optional success message.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing success.</returns>
        public static ApiResponse<T> SuccessResponse(
            T? data,
            string message = "Operation executed successfully")
            => new()
            {
                Success = true,
                Message = message,
                Data = data
            };

        /// <summary>
        /// Creates a successful API response with only a message (no data).
        /// </summary>
        /// <param name="message">Optional success message.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing success.</returns>
        public static ApiResponse<T> SuccessResponse(
            string message = "Operation executed successfully")
            => new()
            {
                Success = true,
                Message = message,
                Data = default
            };

        /// <summary>
        /// Creates an error API response with a message and optional error details.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errors">Optional object containing error details.</param>
        /// <param name="executionTimeMs">Optional execution time in milliseconds.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing failure.</returns>
        public static ApiResponse<T> ErrorResponse(
            string message,
            object? errors = null,
            long? executionTimeMs = null)
            => new()
            {
                Success = false,
                Message = message,
                Data = default
            };
    }
}