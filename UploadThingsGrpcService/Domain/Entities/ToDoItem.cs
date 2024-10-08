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

        public override bool Equals(object? obj)
        {
            if (obj is ToDoItem todoitem)
            {
                return Id == todoitem.Id && Title == todoitem.Title && Description == todoitem.Description && ToDoStatus == todoitem.ToDoStatus;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, ToDoStatus);
        }
    }
}
