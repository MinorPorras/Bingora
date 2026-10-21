using Bingora.Application.DTOs.Pattern;
using FluentValidation;

namespace Bingora.Application.Validators;

public class CreateUpdatePatternRequestValidator : AbstractValidator<CreateUpdatePatternRequestDto>
{
    public CreateUpdatePatternRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("El nombre del patrón es requerido")
            .MaximumLength(50).WithMessage("El nombre debe de tener menos de 50  caracteres");

        RuleFor(p => p.GridRepresentation)
            .NotEmpty().WithMessage("El grid de representación es requerido")
            .Matches(@"^[01]{25}$")
            .WithMessage("La cuadrícula debe ser una cadena exacta de caracteres compuesta solo por '0' y '1'.");
    }
}