namespace BackGazprom.Models.Generic.Responses;

public class ApiResponse
{
    public int Code { get; set; } = 200;
    public object? Payload { get; set; }
    public Exception? Error { get; set; }
    public ApiResponse(object obj)
    {
        Payload = obj;
    }
    public ApiResponse(Exception e, int code = 500)
    {
        Error = e;
    }
}