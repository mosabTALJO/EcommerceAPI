using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EcommerceAPI.Core.UserAddresses.Commands
{
    public class CreateUserAddressCommand : IRequest<RequestResult<int>>
    {
        public string AppUserId { get; set; } = string.Empty;
        public required string Name { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string UserId { get; set; } = string.Empty;
        public static bool IsPhoneNumber(string number) =>
                Regex.Match(number, @"^(\+[0-9]{1,12})$").Success;
        public class CreateUserAddressCommandHandler : IRequestHandler<CreateUserAddressCommand, RequestResult<int>>
        {
            private readonly IUserAddressService _userAddressService;
            private readonly IUnitOfWork _unitOfWork;
            public CreateUserAddressCommandHandler(IUserAddressService userAddressService, IUnitOfWork unitOfWork)
            {
                _userAddressService = userAddressService;
                _unitOfWork = unitOfWork;
            }

            public async Task<RequestResult<int>> Handle(CreateUserAddressCommand command, CancellationToken cancellationToken)
            {
                var ua = new UserAddress
                {
                    AppUserId = command.UserId,
                    Name = command.Name,
                    CityId = command.CityId,
                    CountryId = command.CountryId,
                    IsDefault = command.IsDefault,
                    FirstName = string.IsNullOrEmpty(command.FirstName) ? command.Name : command.FirstName,
                    LastName = string.IsNullOrEmpty(command.LastName) ? string.Empty : command.LastName,
                    Lat = command.Lat,
                    Lng = command.Lng,
                    PhoneNumber = command.PhoneNumber,
                    FullAddress = command.FullAddress, 
                };

                if (!IsPhoneNumber(command.PhoneNumber))
                {
                    var lang = CultureInfo.CurrentCulture?.TwoLetterISOLanguageName ?? "en";
                    return new RequestResult<int>(ActionResultType.BadRequest, lang == "en" ? "Phone number is incorrect" : "رقم الهاتف غير صحيح");
                }
                if (command.IsDefault)
                {
                    foreach (var add in _userAddressService.Queryable().Where(x => x.AppUserId == command.UserId && x.IsDefault).ToArray())
                    {
                        add.IsDefault = false;
                        _userAddressService.Update(add);
                    }
                }
                _userAddressService.Insert(ua);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                    return new RequestResult<int>(ua.Id);
                return new RequestResult<int>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
