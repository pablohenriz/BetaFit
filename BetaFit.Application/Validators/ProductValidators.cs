using BetaFit.Application.DTOs.Product;
using FluentValidation;

namespace BetaFit.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("O preço não pode ser negativo.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gênero inválido.");
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("O preço não pode ser negativo.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gênero inválido.");
    }
}
