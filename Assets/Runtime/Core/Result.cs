namespace Runtime.Core
{
    public class Result
    {
        public bool Success;
        public string Message;
        
        public static Result SuccessResult => new() {Success = true};
        public static Result FailedResult(string message) => new() {Success = false, Message = message};
    }
}