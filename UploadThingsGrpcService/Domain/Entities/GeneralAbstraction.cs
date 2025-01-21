namespace UploadThingsGrpcService.Domain.Entities
{
    public class CurrentIdentity
    {
        public int Id { get; set; }
    }

    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
}
