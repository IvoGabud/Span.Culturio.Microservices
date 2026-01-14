using FluentValidation;
using Span.Culturio.Users.Models.DTOs;

namespace Span.Culturio.Users.Validators
{
    public class GetUsersQueryDtoValidator : AbstractValidator<GetUsersQueryDto>
    {
        public GetUsersQueryDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageSize must be greater than or equal to 1")
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must not exceed 100");
        }
    }
}
