using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CollectionApp.Application.ViewModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
// using System.Linq.Dynamic.Core;
using CollectionApp.Application.Common;

namespace Tracker.Money.Extensions
{
    /// <summary>
    /// Extension methods for standardized API responses
    /// </summary>
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Create a successful API response with 200 status code
        /// </summary>
        public static ActionResult<ApiResponse<T>> OkApiResponse<T>(this ControllerBase controller, T data, string message = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            return controller.Ok(response);
        }

        /// <summary>
        /// Create a successful API response with 201 status code
        /// </summary>
        public static ActionResult<ApiResponse<T>> CreatedApiResponse<T>(this ControllerBase controller, T data, string location, string message = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            controller.Response.Headers.Add("Location", location);
            return controller.Created(location, response);
        }

        /// <summary>
        /// Create a successful API response with 202 status code
        /// </summary>
        public static ActionResult<ApiResponse<T>> AcceptedApiResponse<T>(this ControllerBase controller, T data, string message = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            return controller.Accepted(response);
        }

        /// <summary>
        /// Create a successful API response with 204 status code
        /// </summary>
        public static ActionResult NoContentApiResponse(this ControllerBase controller, string message = null)
        {
            var requestId = GenerateRequestId();
            AddApiHeaders(controller.Response, requestId);
            return controller.NoContent();
        }

        /// <summary>
        /// Create a paginated API response
        /// </summary>
        public static ActionResult<PagedApiResponse<T>> OkPagedApiResponse<T>(this ControllerBase controller, PagedResult<T> pagedData, string message = null)
        {
            var response = new PagedApiResponse<T>
            {
                Success = true,
                Data = pagedData.Items, // Change from pagedData to pagedData.Items
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                TotalCount = pagedData.TotalCount,
                PageNumber = pagedData.PageNumber,
                PageSize = pagedData.PageSize,
                TotalPages = pagedData.TotalPages,
                HasNextPage = pagedData.HasNextPage,
                HasPreviousPage = pagedData.HasPreviousPage
            };

            AddApiHeaders(controller.Response, response.RequestId);
            return controller.Ok(response);
        }

        /// <summary>
        /// Create a bulk operation API response
        /// </summary>
        public static ActionResult<ApiResponse<BulkOperationResponse>> OkBulkApiResponse(this ControllerBase controller, BulkUpdateResultVM result, string message = null)
        {
            var bulkResponse = new BulkOperationResponse
            {
                TotalRequested = result.TotalRequested,
                TotalProcessed = result.TotalProcessed,
                TotalSuccessful = result.TotalSuccessful,
                TotalFailed = result.TotalFailed,
                Results = result.Results,
                Messages = result.Messages,
                CompletedAt = DateTime.UtcNow
            };

            var response = new ApiResponse<BulkOperationResponse>
            {
                Success = true,
                Data = bulkResponse,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            return controller.Ok(response);
        }

        /// <summary>
        /// Create a partial success bulk operation response
        /// </summary>
        public static ActionResult<ApiResponse<BulkOperationResponse>> PartialSuccessBulkApiResponse(this ControllerBase controller, BulkUpdateResultVM result, string message = null)
        {
            var bulkResponse = new BulkOperationResponse
            {
                TotalRequested = result.TotalRequested,
                TotalProcessed = result.TotalProcessed,
                TotalSuccessful = result.TotalSuccessful,
                TotalFailed = result.TotalFailed,
                Results = result.Results,
                Messages = result.Messages,
                CompletedAt = DateTime.UtcNow
            };

            var response = new ApiResponse<BulkOperationResponse>
            {
                Success = result.TotalSuccessful > 0,
                Data = bulkResponse,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            return controller.Ok(response);
        }

        /// <summary>
        /// Create a file download API response
        /// </summary>
        public static ActionResult FileApiResponse(this ControllerBase controller, byte[] fileBytes, string contentType, string fileName)
        {
            var requestId = GenerateRequestId();
            AddApiHeaders(controller.Response, requestId);
            controller.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return controller.File(fileBytes, contentType, fileName);
        }

        /// <summary>
        /// Create a streaming file API response
        /// </summary>
        public static ActionResult StreamApiResponse(this ControllerBase controller, Stream fileStream, string contentType, string fileName)
        {
            var requestId = GenerateRequestId();
            AddApiHeaders(controller.Response, requestId);
            controller.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return controller.File(fileStream, contentType, fileName);
        }

        /// <summary>
        /// Create a cacheable API response
        /// </summary>
        public static ActionResult<ApiResponse<T>> CacheableApiResponse<T>(this ControllerBase controller, T data, TimeSpan cacheDuration, string message = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            controller.Response.Headers.Add("Cache-Control", $"public, max-age={cacheDuration.TotalSeconds}");
            return controller.Ok(response);
        }

        /// <summary>
        /// Create a conditional API response with ETag
        /// </summary>
        public static ActionResult<ApiResponse<T>> ConditionalApiResponse<T>(this ControllerBase controller, T data, string etag, string message = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId()
            };

            AddApiHeaders(controller.Response, response.RequestId);
            controller.Response.Headers.Add("ETag", $"\"{etag}\"");
            return controller.Ok(response);
        }

        #region Error Response Extensions

        /// <summary>
        /// Create a bad request API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> BadRequestApiResponse(this ControllerBase controller, string message, Dictionary<string, string[]> errors = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message,
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Title = "Validation Error",
                    Detail = message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Instance = controller.Request.Path,
                    Errors = errors ?? new Dictionary<string, string[]>()
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.BadRequest(errorResponse);
        }

        /// <summary>
        /// Create an unauthorized API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> UnauthorizedApiResponse(this ControllerBase controller, string message = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message ?? "Unauthorized access",
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "UNAUTHORIZED",
                    Title = "Unauthorized",
                    Detail = message ?? "Authentication is required to access this resource",
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Instance = controller.Request.Path
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.Unauthorized();
        }

        /// <summary>
        /// Create a forbidden API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> ForbiddenApiResponse(this ControllerBase controller, string message = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message ?? "Access forbidden",
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "FORBIDDEN",
                    Title = "Forbidden",
                    Detail = message ?? "You do not have permission to access this resource",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Instance = controller.Request.Path
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.Forbid();
        }

        /// <summary>
        /// Create a not found API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> NotFoundApiResponse(this ControllerBase controller, string message = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message ?? "Resource not found",
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "NOT_FOUND",
                    Title = "Not Found",
                    Detail = message ?? "The requested resource was not found",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Instance = controller.Request.Path
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.NotFound(errorResponse);
        }

        /// <summary>
        /// Create a conflict API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> ConflictApiResponse(this ControllerBase controller, string message = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message ?? "Resource conflict",
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "CONFLICT",
                    Title = "Conflict",
                    Detail = message ?? "The request conflicts with the current state of the resource",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Instance = controller.Request.Path
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.Conflict(errorResponse);
        }

        /// <summary>
        /// Create an internal server error API response
        /// </summary>
        public static ActionResult<ApiErrorResponse> InternalServerErrorApiResponse(this ControllerBase controller, string message = null)
        {
            var errorResponse = new ApiErrorResponse
            {
                Success = false,
                Message = message ?? "Internal server error",
                Timestamp = DateTime.UtcNow,
                RequestId = GenerateRequestId(),
                Error = new ApiError
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Title = "Internal Server Error",
                    Detail = message ?? "An unexpected error occurred while processing the request",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Instance = controller.Request.Path
                }
            };

            AddApiHeaders(controller.Response, errorResponse.RequestId);
            return controller.StatusCode(500, errorResponse);
        }

        #endregion

        #region Validation Response Extensions

        /// <summary>
        /// Create a validation error API response from ModelState
        /// </summary>
        public static ActionResult<ApiErrorResponse> ValidationErrorApiResponse(this ControllerBase controller, ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var kvp in modelState)
            {
                if (kvp.Value.Errors.Count > 0)
                {
                    errors[kvp.Key] = kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }

            return controller.BadRequestApiResponse("Validation failed", errors);
        }

        /// <summary>
        /// Create a validation error API response from ValidationResults
        /// </summary>
        public static ActionResult<ApiErrorResponse> ValidationErrorApiResponse(this ControllerBase controller, List<ValidationResult> validationResults)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var result in validationResults)
            {
                var fieldName = result.MemberNames.FirstOrDefault() ?? "General";
                if (!errors.ContainsKey(fieldName))
                {
                    errors[fieldName] = new string[0];
                }

                var currentErrors = errors[fieldName].ToList();
                currentErrors.Add(result.ErrorMessage);
                errors[fieldName] = currentErrors.ToArray();
            }

            return controller.BadRequestApiResponse("Validation failed", errors);
        }

        #endregion

        #region Exception Handling Extensions

        /// <summary>
        /// Handle API exceptions and convert to appropriate responses
        /// </summary>
        public static ActionResult<ApiResponse<T>> HandleApiException<T>(this ControllerBase controller, Exception ex, ILogger logger, string operation)
        {
            var requestId = GenerateRequestId();
            logger.LogError(ex, "Error in {Operation} operation. RequestId: {RequestId}", operation, requestId);

            var errorResponse = ex switch
            {
                ArgumentException => BadRequestApiResponse<T>(controller, ex.Message),
                ValidationException => BadRequestApiResponse<T>(controller, ex.Message),
                KeyNotFoundException => NotFoundApiResponse<T>(controller, ex.Message),
                InvalidOperationException => BadRequestApiResponse<T>(controller, ex.Message),
                UnauthorizedAccessException => UnauthorizedApiResponse<T>(controller, ex.Message),
                _ => InternalServerErrorApiResponse<T>(controller, "An unexpected error occurred")
            };

            return errorResponse;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Generate a unique request identifier
        /// </summary>
        public static string GenerateRequestId()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Extract correlation ID from request headers
        /// </summary>
        public static string GetCorrelationId(this HttpContext context)
        {
            return context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? GenerateRequestId();
        }

        /// <summary>
        /// Add standard API headers to response
        /// </summary>
        public static void AddApiHeaders(this HttpResponse response, string requestId)
        {
            response.Headers.Add("X-Request-ID", requestId);
            response.Headers.Add("X-API-Version", "1.0");
            response.Headers.Add("X-Response-Time", DateTime.UtcNow.ToString("O"));
        }

        /// <summary>
        /// Create a ProblemDetails object for error responses
        /// </summary>
        public static ProblemDetails CreateProblemDetails(this ControllerBase controller, string title, string detail, int statusCode)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Type = $"https://tools.ietf.org/html/rfc7231#section-6.5.{statusCode}",
                Instance = controller.Request.Path
            };
        }

        #endregion


        #region Error Response Extensions

/// <summary>
/// Create a generic bad request API response
/// </summary>
public static ActionResult<ApiResponse<T>> BadRequestApiResponse<T>(this ControllerBase controller, string message, Dictionary<string, string[]> errors = null)
{
    var response = new ApiResponse<T>
    {
        Success = false,
        Data = default,
        Message = message,
        Timestamp = DateTime.UtcNow,
        RequestId = GenerateRequestId(),
        Error = new ApiError
        {
            Code = "VALIDATION_ERROR",
            Title = "Validation Error",
            Detail = message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = controller.Request.Path,
            Errors = errors ?? new Dictionary<string, string[]>()
        }
    };

    AddApiHeaders(controller.Response, response.RequestId);
    return controller.BadRequest(response);
}

/// <summary>
/// Create a generic unauthorized API response
/// </summary>
public static ActionResult<ApiResponse<T>> UnauthorizedApiResponse<T>(this ControllerBase controller, string message = null)
{
    var response = new ApiResponse<T>
    {
        Success = false,
        Data = default,
        Message = message ?? "Unauthorized access",
        Timestamp = DateTime.UtcNow,
        RequestId = GenerateRequestId(),
        Error = new ApiError
        {
            Code = "UNAUTHORIZED",
            Title = "Unauthorized",
            Detail = message ?? "Authentication is required to access this resource",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Instance = controller.Request.Path
        }
    };

    AddApiHeaders(controller.Response, response.RequestId);
    return controller.Unauthorized(response);
}

/// <summary>
/// Create a generic not found API response
/// </summary>
public static ActionResult<ApiResponse<T>> NotFoundApiResponse<T>(this ControllerBase controller, string message = null)
{
    var response = new ApiResponse<T>
    {
        Success = false,
        Data = default,
        Message = message ?? "Resource not found",
        Timestamp = DateTime.UtcNow,
        RequestId = GenerateRequestId(),
        Error = new ApiError
        {
            Code = "NOT_FOUND",
            Title = "Not Found",
            Detail = message ?? "The requested resource was not found",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Instance = controller.Request.Path
        }
    };

    AddApiHeaders(controller.Response, response.RequestId);
    return controller.NotFound(response);
}

/// <summary>
/// Create a generic internal server error API response
/// </summary>
public static ActionResult<ApiResponse<T>> InternalServerErrorApiResponse<T>(this ControllerBase controller, string message = null)
{
    var response = new ApiResponse<T>
    {
        Success = false,
        Data = default,
        Message = message ?? "Internal server error",
        Timestamp = DateTime.UtcNow,
        RequestId = GenerateRequestId(),
        Error = new ApiError
        {
            Code = "INTERNAL_SERVER_ERROR",
            Title = "Internal Server Error",
            Detail = message ?? "An unexpected error occurred while processing the request",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Instance = controller.Request.Path
        }
    };

    AddApiHeaders(controller.Response, response.RequestId);
    return controller.StatusCode(500, response);
}

#endregion
    }
}