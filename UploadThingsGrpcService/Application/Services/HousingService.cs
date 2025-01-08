using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.HousingLocationProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class HousingLocationServices(IUnitOfWork unitofWorkRepository) : HousingLocationService.HousingLocationServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;

        public ReadHousingLocationResponse ApplyFieldMask(ReadHousingLocationResponse fullData, FieldMask fieldMask)
        {
            ReadHousingLocationResponse selectedData = new();
            foreach (string? field in fieldMask.Paths)
            {
                switch (field)
                {
                    case "id":
                        selectedData.Id = fullData.Id;
                        break;
                    case "name":
                        selectedData.Name = fullData.Name;
                        break;
                    case "city":
                        selectedData.City = fullData.City;
                        break;
                    case "state":
                        selectedData.State = fullData.State;
                        break;
                    case "photo":
                        selectedData.Photo = fullData.Photo;
                        break;
                    case "availableUnits":
                        selectedData.AvailableUnits = fullData.AvailableUnits;
                        break;
                    case "wifi":
                        selectedData.Wifi = fullData.Wifi;
                        break;
                    case "laundry":
                        selectedData.Laundry = fullData.Laundry;
                        break;
                }
            }
            return selectedData;
        }

        // Test to use RAFunction HousingLocation Repository method The other function is from General Repository
        public override async Task<TestResponse> RAFunction(TestRequest request, ServerCallContext context)
        {
            TestResponse test = new()
            {
                Hello = await _unitofWorkRepository.HousingLocationRepository.Test()
            };
            return await Task.FromResult(test);
        }

        public override async Task<CreateHousingLocationResponse> CreateHousingLocation(CreateHousingLocationRequest request, ServerCallContext context)
        {
            if (
                    request.Name == string.Empty ||
                    request.City == string.Empty ||
                    request.State == string.Empty ||
                    request.Photo == string.Empty
                )
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));
            }

            HousingLocation housingLocation = new()
            {
                Name = request.Name,
                City = request.City,
                State = request.State,
                Photo = request.Photo,
                AvailableUnits = request.AvailableUnits,
                Wifi = request.Wifi,
                Laundry = request.Laundry
            };
            await _unitofWorkRepository.HousingLocationRepository.AddAsync(housingLocation);
            return await Task.FromResult(new CreateHousingLocationResponse { Id = housingLocation.Id });
        }

        public override async Task<ReadHousingLocationResponse> ReadHousingLocation(ReadHousingLocationRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            HousingLocation? housingLocationItem = await _unitofWorkRepository.HousingLocationRepository.GetByIdAsync(request.Id);
            if (housingLocationItem != null)
            {
                ReadHousingLocationResponse readfulldata = new()
                {
                    Id = housingLocationItem.Id,
                    Name = housingLocationItem?.Name,
                    State = housingLocationItem?.State,
                    City = housingLocationItem?.City,
                    Photo = housingLocationItem?.Photo,
                    AvailableUnits = housingLocationItem?.AvailableUnits ?? 0,
                    Wifi = housingLocationItem?.Wifi ?? false,
                    Laundry = housingLocationItem?.Laundry ?? false
                };
                ReadHousingLocationResponse selectedData = ApplyFieldMask(readfulldata, request.DataThatNeeded);

                return await Task.FromResult(new ReadHousingLocationResponse
                {
                    Id = selectedData.Id,
                    Name = selectedData?.Name,
                    State = selectedData?.State,
                    City = selectedData?.City,
                    Photo = selectedData?.Photo,
                    AvailableUnits = selectedData?.AvailableUnits ?? 0,
                    Wifi = selectedData?.Wifi ?? false,
                    Laundry = selectedData?.Laundry ?? false
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListHousingLocation(GetAllRequest request, ServerCallContext context)
        {
            GetAllResponse response = new();
            IEnumerable<HousingLocation> housingLocationItem = await _unitofWorkRepository.HousingLocationRepository.GetAllAsync();
            foreach (HousingLocation? housingLocation in housingLocationItem)
            {
                response.HousingLocationData.Add(new ReadHousingLocationResponse
                {
                    Id = housingLocation.Id,
                    Name = housingLocation?.Name,
                    State = housingLocation?.State,
                    City = housingLocation?.City,
                    Photo = housingLocation?.Photo,
                    AvailableUnits = housingLocation?.AvailableUnits ?? 0,
                    Wifi = housingLocation?.Wifi ?? false,
                    Laundry = housingLocation?.Laundry ?? false
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateHousingLocationResponse> UpdateHousingLocation(UpdateHousingLocationRequest request, ServerCallContext context)
        {
            if (request.Name == string.Empty || request.City == string.Empty || request.State == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Some of the data might be empty."));

            HousingLocation housingLocationItem = await _unitofWorkRepository.HousingLocationRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            housingLocationItem.Name = request.Name;
            housingLocationItem.City = request.City;
            housingLocationItem.State = request.State;
            housingLocationItem.Photo = request.Photo;
            housingLocationItem.AvailableUnits = request.AvailableUnits;
            housingLocationItem.Wifi = request.Wifi;
            housingLocationItem.Laundry = request.Laundry;

            await _unitofWorkRepository.HousingLocationRepository.UpdateAsync(housingLocationItem);
            return await Task.FromResult(new UpdateHousingLocationResponse { Id = request.Id });
        }

        public override async Task<DeleteHousingLocationResponse> DeleteHousingLocation(DeleteHousingLocationRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            HousingLocation housingLocationItem = await _unitofWorkRepository.HousingLocationRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            await _unitofWorkRepository.HousingLocationRepository.DeleteAsync(housingLocationItem.Id);

            return await Task.FromResult(new DeleteHousingLocationResponse { Id = request.Id });
        }
    }
}
