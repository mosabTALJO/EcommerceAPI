using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using EcommerceAPI.Shared.Helpers;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.Shared.Helpers.Abstractions;

namespace EcommerceAPI.Core.EmailPromotions.Commands
{
    public class CreateEmailPromotionCommand : IRequest<RequestResult<int>>
    {
        public string TestEmail { get; set; }
        public string Subject { get; set; }
        public PromotionFilter Filter { get; set; }
        public DateTime? FilterDate { get; set; }
        public string HtmlText { get; set; }
        public int CountryId { get; set; }
        public class CreateEmailPromotionCommandHandler : IRequestHandler<CreateEmailPromotionCommand, RequestResult<int>>
        {
            private readonly IEmailPromotionService _emailPromotionService;
            private readonly ICountryService _countryService;
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IAppEmailSender _emailSender;

            public CreateEmailPromotionCommandHandler(IEmailPromotionService emailPromotionService, IMapper mapper, IUnitOfWork unitOfWork, IAppEmailSender emailSender, UserManager<AppUser> userManager, ICountryService countryService)
            {
                _emailPromotionService = emailPromotionService;
                _mapper = mapper;
                _unitOfWork = unitOfWork;
                _emailSender = emailSender;
                _userManager = userManager;
                _countryService = countryService;
            }

            public async Task<RequestResult<int>> Handle(CreateEmailPromotionCommand command, CancellationToken cancellationToken)
            {
                var email = _mapper.Map<EmailPromotion>(command);
                _emailPromotionService.Insert(email);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    if (!string.IsNullOrEmpty(command.TestEmail))
                    {
                        await _emailSender.SendEmailAsync(new string[] { command.TestEmail, }, command.Subject, command.HtmlText, false);
                    }
                    else
                    {
                        var c = _countryService.Queryable().FirstOrDefault(x => x.Id == command.CountryId);
                        var customers = _userManager.Users
                            .Where(x => x.UserType == UserTypes.Customer)
                            .Where(x => x.IsActive && !string.IsNullOrEmpty(x.Email))
                            .Where(x => command.CountryId == 0 || x.Country == c.Code || x.Country == c.NameEn || x.Country == c.NameAr || x.Country == c.CurrencyCode)
                            .Select(x => x.Email.Trim())
                            .ToArray();

                        var allEmails = customers.Where(x => IsValid(x)).ToArray();
                        while (allEmails.Any())
                        {
                            var emails = allEmails.Length > 500 ? allEmails.Take(500).ToArray() : allEmails;
                            var r = await _emailSender.SendEmailAsync(emails, command.Subject, command.HtmlText, true);
                            if (r.StatusCode != System.Net.HttpStatusCode.OK)
                                throw new Exception(r.StatusCode + " body: " + r.Body);

                            allEmails = allEmails.Except(emails).ToArray();
                        }
                    }
                    return new RequestResult<int>(email.Id);
                }
                return new RequestResult<int>(ActionResultType.BadRequest, "An error occured when saving");
            }

            private static bool IsValid(string email)
            {
                try
                {
                    var emailAddress = new MailAddress(email);
                    return true;
                }
                catch { }
                return false;
            }
        }
    }
}
