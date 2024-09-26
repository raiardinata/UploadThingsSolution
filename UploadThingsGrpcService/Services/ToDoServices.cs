using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UploadThingsGrpcService.Data;
using UploadThingsGrpcService.Models;

namespace UploadThingsGrpcService.Services
{
    public class ToDoServices : TodoIt.TodoItBase
    {
        private readonly MSSQLContext _dbContext;
        ReadFullDataToDo readfulldatatodo = new ReadFullDataToDo();

        private ReadFullDataToDo ApplyFieldMask(ReadFullDataToDo fulldata, FieldMask fieldMask)
        {
            var selectedData = new ReadFullDataToDo();
            foreach (var field in fieldMask.Paths)
            {
                switch (field)
                {
                    case "id":
                        selectedData.Id = fulldata.Id;
                        break;
                    case "title":
                        selectedData.Title = fulldata.Title;
                        break;
                    case "description":
                        selectedData.Description = fulldata.Description;
                        break;
                    case "todostatus":
                        selectedData.ToDoStatus = fulldata.ToDoStatus;
                        break;
                }
            }
            return selectedData;
        }

        public ToDoServices(MSSQLContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateTodoResponse> CreateToDo(CreateTodoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = new ToDoItem { Title = request.Title, Description = request.Description };

            await _dbContext.AddAsync(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateTodoResponse { Id = toDoItem.Id });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (toDoItem != null)
            {
                readfulldatatodo = new ReadFullDataToDo
                {
                    Id = toDoItem.Id,
                    Title = toDoItem?.Title,
                    Description = toDoItem?.Description,
                    ToDoStatus = toDoItem?.ToDoStatus,
                };
                var selectedData = ApplyFieldMask(readfulldatatodo, request.DataThatNeeded);

                return await Task.FromResult(new ReadToDoResponse
                {
                    TodoData = selectedData,
                });
            }



            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItem = await _dbContext.ToDoItems.ToListAsync();
            foreach (var todo in toDoItem)
            {
                readfulldatatodo = new ReadFullDataToDo
                {
                    Id = todo.Id,
                    Title = todo?.Title,
                    Description = todo?.Description,
                    ToDoStatus = todo?.ToDoStatus,
                };
                response.TodoData.Add(new ReadToDoResponse
                {
                    TodoData = readfulldatatodo,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Title == string.Empty || request.Title == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.ToDoStatus;

            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new UpdateToDoResponse { Id = request.Id });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            _dbContext.Remove(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse { Id = request.Id });
        }
    }
}