using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.UserProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class UserServices(IUnitOfWork unitofWorkRepository) : UserService.UserServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;

        ReadUserResponse readFullDataUser = new();
        public ReadUserResponse ApplyFieldMask(ReadUserResponse fullData, FieldMask fieldMask)
        {
            ReadUserResponse selectedData = new();
            foreach (var field in fieldMask.Paths)
            {
                switch (field)
                {
                    case "id":
                        selectedData.Id = fullData.Id;
                        break;
                    case "name":
                        selectedData.Name = fullData.Name;
                        break;
                    case "email":
                        selectedData.Email = fullData.Email;
                        break;
                }
            }
            return selectedData;
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            if (request == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));
            }

            User user = new() { Name = request.Name, Email = request.Email };
            await _unitofWorkRepository.UserRepository.AddAsync(user);
            return await Task.FromResult(new CreateUserResponse { Id = user.Id });
        }

        public override async Task<ReadUserResponse> ReadUser(ReadUserRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            var user = await _unitofWorkRepository.UserRepository.GetByIdAsync(request.Id);
            if (user != null)
            {
                readFullDataUser = new ReadUserResponse
                {
                    Id = user.Id,
                    Name = user?.Name,
                    Email = user?.Email,
                };
                var selectedData = ApplyFieldMask(readFullDataUser, request.DataThatNeeded);

                return await Task.FromResult(new ReadUserResponse
                {
                    Id = selectedData.Id,
                    Email = selectedData.Email,
                    Name = selectedData.Name,
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"Failed to read User with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListUser(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var users = await _unitofWorkRepository.UserRepository.GetAllAsync();
            foreach (var user in users)
            {
                response.UserData.Add(new ReadUserResponse
                {
                    Id = user.Id,
                    Name = user?.Name,
                    Email = user?.Email,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            if (request.Id <= 0 || request.Name == string.Empty || request.Email == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            User user = await _unitofWorkRepository.UserRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            user.Name = request.Name;
            user.Email = request.Email;

            await _unitofWorkRepository.UserRepository.UpdateAsync(user);
            return await Task.FromResult(new UpdateUserResponse { Id = request.Id });
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            User user = await _unitofWorkRepository.UserRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}")); ;

            await _unitofWorkRepository.UserRepository.DeleteAsync(user.Id);

            return await Task.FromResult(new DeleteUserResponse { Id = request.Id });
        }
    }
}
