using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Domain.Entities
{
    // ToDoItem model inherit IEntity
    public class ToDoItem : IEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ToDoStatus { get; set; } = "NEW";
    }
}
