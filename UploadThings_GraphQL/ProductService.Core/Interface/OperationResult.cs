namespace ProductService.Core.Interface
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static OperationResult SuccessResult() => new() { Success = true };
        public static OperationResult FailureResult(string message) => new() { Success = false, Message = message };
    }
}
