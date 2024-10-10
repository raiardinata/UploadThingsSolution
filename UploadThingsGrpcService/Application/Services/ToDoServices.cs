using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.ToDoProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class ToDoServices(IUnitOfWork unitofWorkRepository) : ToDoService.ToDoServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;

        ReadToDoResponse readfulldatatodo = new();
        private static ReadToDoResponse ApplyFieldMask(ReadToDoResponse fulldata, FieldMask fieldMask)
        {
            var selectedData = new ReadToDoResponse();
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

            await _unitofWorkRepository.ToDoRepository.AddAsync(toDoItem);

            return await Task.FromResult(new CreateTodoResponse { Id = toDoItem.Id });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            var toDoItem = await _unitofWorkRepository.ToDoRepository.GetByIdAsync(request.Id);
            if (toDoItem != null)
            {
                readfulldatatodo = new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    Title = toDoItem?.Title,
                    Description = toDoItem?.Description,
                    ToDoStatus = toDoItem?.ToDoStatus,
                };
                var selectedData = ApplyFieldMask(readfulldatatodo, request.DataThatNeeded);

                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = selectedData.Id,
                    Title = selectedData?.Title,
                    Description = selectedData?.Description,
                    ToDoStatus = selectedData?.ToDoStatus,
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItem = await _unitofWorkRepository.ToDoRepository.GetAllAsync();
            foreach (var todo in toDoItem)
            {
                response.TodoData.Add(new ReadToDoResponse
                {
                    Id = todo.Id,
                    Title = todo?.Title,
                    Description = todo?.Description,
                    ToDoStatus = todo?.ToDoStatus,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Title == string.Empty || request.Title == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            ToDoItem toDoItem = await _unitofWorkRepository.ToDoRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.ToDoStatus;

            await _unitofWorkRepository.ToDoRepository.UpdateAsync(toDoItem);
            return await Task.FromResult(new UpdateToDoResponse { Id = request.Id });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            var toDoItem = await _unitofWorkRepository.ToDoRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            await _unitofWorkRepository.ToDoRepository.DeleteAsync(toDoItem.Id);

            return await Task.FromResult(new DeleteToDoResponse { Id = request.Id });
        }
    }
}