using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using System.Globalization;

namespace EcommerceAPI.Core.UserAddresses.Commands
{
    public class UpdateUserAddressCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public required string Name { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string UserId { get; set; } = string.Empty;
        public class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand, RequestResult<bool>>
        {
            private readonly IUserAddressService _userAddressService;
            private readonly IUnitOfWork _unitOfWork;
            public UpdateUserAddressCommandHandler(IUserAddressService userAddressService, IUnitOfWork unitOfWork)
            {
                _userAddressService = userAddressService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(UpdateUserAddressCommand command, CancellationToken cancellationToken)
            {
                var ua = _userAddressService.Queryable().FirstOrDefault(x => x.Id == command.Id && x.AppUserId == command.UserId);
                if (ua != null)
                {
                    ua.Name = command.Name;
                    ua.CountryId = command.CountryId;
                    ua.CityId = command.CityId;
                    ua.FullAddress = command.FullAddress;
                    ua.Lat = command.Lat;
                    ua.Lng = command.Lng;
                    ua.FirstName = string.IsNullOrEmpty(command.FirstName) ? command.Name : command.FirstName;
                    ua.LastName = string.IsNullOrEmpty(command.LastName) ? string.Empty : command.LastName;
                    ua.PhoneNumber = command.PhoneNumber;
                    ua.IsDefault = command.IsDefault;

                    if (!CreateUserAddressCommand.IsPhoneNumber(command.PhoneNumber))
                    {
                        var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                        return new RequestResult<bool>(ActionResultType.BadRequest, lang == "en" ? "Phone number is incorrect" : "رقم الهاتف غير صحيح");
                    }

                    _userAddressService.Update(ua);
                    if (ua.IsDefault)
                    {
                        foreach (var add in _userAddressService.Queryable().Where(x => x.Id != command.Id && x.AppUserId == command.UserId && x.IsDefault).ToArray())
                        {
                            add.IsDefault = false;
                            _userAddressService.Update(add);
                        }
                    }

                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                else
                    return new RequestResult<bool>(ActionResultType.NotFound, "IdError");
                return new RequestResult<bool>(ActionResultType.NotFound, "Error");
            }
        }
    }
}
