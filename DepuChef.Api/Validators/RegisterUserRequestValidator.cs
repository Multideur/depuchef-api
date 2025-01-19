using DepuChef.Application.Models.User;
using FluentValidation;

namespace DepuChef.Api.Validators;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.FirstName)
            .NotEmpty();
        RuleFor(x => x.LastName)
            .NotEmpty();
        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty();
        RuleFor(x => x.ChefPreference)
            .NotEmpty();
        RuleFor(x => x.Subscription)
            .NotEmpty();
    }
}
