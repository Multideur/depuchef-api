using DepuChef.Application.Models;
using FluentValidation;

namespace DepuChef.Api.Validators
{
    public class RecipeRequestValidator : AbstractValidator<RecipeRequest>
    {
        public RecipeRequestValidator()
        {
            When(x => x.Text is null, () =>
            {
                RuleFor(x => x.Image)
                    .NotEmpty()
                    .WithMessage("Image is required when text is not provided.")
                    .Must(x => x!.Length <= 50_000_000)
                    .WithMessage("Image is too large.")
                    .Must(x => x!.ContentType.Contains("image"))
                    .WithMessage("File is not an image.");
            });
            When(x => x.Image is null, () =>
            {
                RuleFor(x => x.Text)
                    .NotEmpty()
                    .WithMessage("Text is required when image is not provided.")
                    .MaximumLength(200)
                    .WithMessage("Text is too long.");
            });
            RuleFor(x => x.UserId)
                .NotEmpty();
            RuleFor(x => x.ConnectionId)
                .NotEmpty();
        }
    }
}