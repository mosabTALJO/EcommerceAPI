using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.UnitOfWork.Abstractions;
using MediatR;

namespace EcommerceAPI.Core.BaseItems.Command
{
    public class DeleteBaseItemCommand : IRequest<RequestResult<bool>>
    {
        public int Id { get; set; }
        public class DeleteBaseItemCommandHandler : IRequestHandler<DeleteBaseItemCommand, RequestResult<bool>>
        {
            private readonly IBaseItemService _baseItemService;
            private readonly IUnitOfWork _unitOfWork;

            public DeleteBaseItemCommandHandler(IBaseItemService baseItemService, IUnitOfWork unitOfWork)
            {
                _baseItemService = baseItemService;
                _unitOfWork = unitOfWork;
            }
            public async Task<RequestResult<bool>> Handle(DeleteBaseItemCommand command, CancellationToken cancellationToken)
            {
                if (await _baseItemService.FindAsync(command.Id) is BaseItem item)
                {
                    _baseItemService.Delete(item);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                        return new RequestResult<bool>(true);
                }
                return new RequestResult<bool>(ActionResultType.BadRequest, "Error");
            }
        }
    }
}
