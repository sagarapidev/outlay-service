namespace OutlayService.Responses;

/// <summary>
/// Generic response wrapper for API responses
/// </summary>
/// <typeparam name="T">The type of data in the response</typeparam>
public class OutlayServiceResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message describing the result of the operation
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The result data returned by the operation
    /// </summary>
    public T? Result { get; set; }

    /// <summary>
    /// Initializes a new instance of the OutlayServiceResponse class
    /// </summary>
    public OutlayServiceResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the OutlayServiceResponse class with success and message
    /// </summary>
    public OutlayServiceResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the OutlayServiceResponse class with success, message and result
    /// </summary>
    public OutlayServiceResponse(bool success, string message, T? result)
    {
        Success = success;
        Message = message;
        Result = result;
    }

    /// <summary>
    /// Creates a successful response with result data
    /// </summary>
    public static OutlayServiceResponse<T> SuccessResponse(T? data, string message = "Operation successful")
    {
        return new OutlayServiceResponse<T>(true, message, data);
    }

    /// <summary>
    /// Creates a successful response without result data
    /// </summary>
    public static OutlayServiceResponse<T> SuccessResponse(string message = "Operation successful")
    {
        return new OutlayServiceResponse<T>(true, message);
    }

    /// <summary>
    /// Creates a failure response
    /// </summary>
    public static OutlayServiceResponse<T> FailureResponse(string message = "Operation failed")
    {
        return new OutlayServiceResponse<T>(false, message);
    }

    /// <summary>
    /// Creates a failure response with result data
    /// </summary>
    public static OutlayServiceResponse<T> FailureResponse(string message, T? data)
    {
        return new OutlayServiceResponse<T>(false, message, data);
    }
}
