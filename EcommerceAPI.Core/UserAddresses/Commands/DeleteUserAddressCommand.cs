using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.UserAddresses.Commands
{
    public class DeleteUserAddressCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public class DeleteUserAddressCommandHandler : IRequestHandler<DeleteUserAddressCommand, RequestResult<bool>>
        {
            private readonly IUserAddressService _userAddressService;
            private readonly IUnitOfWork _unitOfWork;
            public DeleteUserAddressCommandHandler(IUserAddressService userAddressService, IUnitOfWork unitOfWork)
            {
                _userAddressService = userAddressService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteUserAddressCommand command, CancellationToken cancellationToken)
            {
                var ua = _userAddressService.Queryable().FirstOrDefault(x => x.Id == command.Id && x.AppUserId == command.UserId);
                if (ua != null)
                {
                    _userAddressService.Delete(ua);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                    {
                        if (ua.IsDefault)
                        {
                            var newDefault = _userAddressService.Queryable().FirstOrDefault(x => x.AppUserId == command.UserId);
                            if (newDefault != null)
                            {
                                newDefault.IsDefault = true;
                                _userAddressService.Update(newDefault);
                                if (await _unitOfWork.SaveChangesAsync() > 0)
                                    return new RequestResult<bool>(true);
                            }
                            else
                                return new RequestResult<bool>(true);
                        }
                        else
                            return new RequestResult<bool>(true);
                    }
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
