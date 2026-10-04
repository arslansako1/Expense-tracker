

public class ServiceResult<T>
{
    public bool Success {get; set;}
    public string ErrorMessage {get; set;} = string.Empty;
    public string Message {get; set;} = string.Empty;
    public T? Data {get; set;}
    public static ServiceResult<T> Ok(T data)
      => new() {Success = true, Data = data};
    public static ServiceResult<T> Fail(string message)
      => new() {Success = false, ErrorMessage = message};

    public static ServiceResult<T> NoContent(string message)
      => new () {Success = true, Message = message};

    internal static ServiceResult<ResetPasswordResponse> Ok(string v)
    {
        throw new NotImplementedException();
    }
}