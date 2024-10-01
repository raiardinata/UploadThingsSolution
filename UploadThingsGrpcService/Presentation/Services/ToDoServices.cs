using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcServic.ToDoService;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsGrpcService.Presentation.Services
{
    public class ToDoServices : TodoIt.TodoItBase
    {
        private readonly IGeneralRepository<ToDoItem> _toDoItemsRepository;
        public ToDoServices(IGeneralRepository<ToDoItem> toDoItemsRepository)
        {
            _toDoItemsRepository = toDoItemsRepository;
        }

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

        public override async Task<CreateTodoResponse> CreateToDo(CreateTodoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = new ToDoItem { Title = request.Title, Description = request.Description };

            await _toDoItemsRepository.AddAsync(toDoItem);

            return await Task.FromResult(new CreateTodoResponse { Id = toDoItem.Id });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            var toDoItem = await _toDoItemsRepository.GetByIdAsync(request.Id);
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
            var toDoItem = await _toDoItemsRepository.GetAllAsync();
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

            var toDoItem = await _toDoItemsRepository.GetByIdAsync(request.Id);
            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.ToDoStatus;

            await _toDoItemsRepository.AddAsync(toDoItem);
            return await Task.FromResult(new UpdateToDoResponse { Id = request.Id });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = await _toDoItemsRepository.GetByIdAsync(request.Id);
            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            await _toDoItemsRepository.DeleteAsync(toDoItem.Id);

            return await Task.FromResult(new DeleteToDoResponse { Id = request.Id });
        }
    }
}