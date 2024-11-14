using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.UserProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class UserServices(IUnitOfWork unitofWorkRepository) : UserService.UserServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
        {
            try
            {
                return _passwordHasher.HashPassword(user, password);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while hashing the password.", ex));
            }
        }

        public bool VerifyPassword(User user, string hashedPassword, string password)
        {
            try
            {
                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
                return result == PasswordVerificationResult.Success;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while verifying the password.", ex));
            }
        }

        ReadUserResponse readFullDataUser = new();
        public ReadUserResponse ApplyFieldMask(ReadUserResponse fullData, FieldMask fieldMask)
        {
            ReadUserResponse selectedData = new();
            foreach (string field in fieldMask.Paths)
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

            User user = new() { Name = request.Name, Email = request.Email, PasswordHashed = request.Passwordhashed };
            user.PasswordHashed = HashPassword(user, user.PasswordHashed);

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
            if (request.Id <= 0 || request.Name == string.Empty || request.Email == string.Empty || request.Passwordhashed == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            User user = await _unitofWorkRepository.UserRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            user.Name = request.Name;
            user.Email = request.Email;
            user.PasswordHashed = HashPassword(user, request.Passwordhashed);

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

        public override async Task<UserLoginResponse> UserLogin(UserLoginRequest request, ServerCallContext context)
        {
            if (request.Email == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Email or Password."));

            User user = new() { Email = request.Email, PasswordHashed = request.Passwordhashed };

            bool methodResponse = await _unitofWorkRepository.UserRepository.UserLogin(user);

            if (!methodResponse)
                return await Task.FromResult(new UserLoginResponse() { Valid = methodResponse });

            return await Task.FromResult(new UserLoginResponse() { Valid = methodResponse });
        }
    }
}
