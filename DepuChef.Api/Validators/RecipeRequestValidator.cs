using DepuChef.Application.Models;
using FluentValidation;

namespace DepuChef.Api.Validators
{
    public class RecipeRequestValidator : AbstractValidator<RecipeRequest>
    {
        public RecipeRequestValidator()
        {
            RuleFor(x => x.Image)
                .NotEmpty()
                .Must(x => x!.Length <= 5_000_000)
                .WithMessage("Image is too large.")
                .Must(x => x!.ContentType.Contains("image"))
                .WithMessage("File is not an image.");
            RuleFor(x => x.UserId)
                .NotEmpty();
            RuleFor(x => x.ConnectionId)
                .NotEmpty();
        }
    }
}