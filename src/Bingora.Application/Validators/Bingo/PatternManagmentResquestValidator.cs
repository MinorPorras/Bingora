using Bingora.Application.DTOs.Bingo;
using FluentValidation;

namespace Bingora.Application.Validators.Bingo;

public class PatternManagementRequestValidator : AbstractValidator<PatternManagementRequestDto>
{
    public PatternManagementRequestValidator()
    {
        RuleFor(d => d.AvailablePrizes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El número de premios disponibles para este patrón no puede ser negativo");
    }
}