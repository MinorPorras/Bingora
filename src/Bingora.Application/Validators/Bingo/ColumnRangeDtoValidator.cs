using Bingora.Application.DTOs.Bingo;
using FluentValidation;

namespace Bingora.Application.Validators.Bingo;

public class ColumnRangeDtoValidator : AbstractValidator<ColumnRangeDto>
{
    public ColumnRangeDtoValidator()
    {
        RuleFor(c => c.ColumnName)
            .NotEmpty().WithMessage("El nombre de la columna es requerido.")
            .MaximumLength(10).WithMessage("El nombre de la columna no puede exceder los 10 caracteres.");

        RuleFor(c => c.MinValue)
            .GreaterThanOrEqualTo(0).WithMessage("El valor mínimo no puede ser negativo.");

        RuleFor(c => c.MaxValue)
            .GreaterThan(c => c.MinValue).WithMessage("El valor máximo debe ser mayor que el valor mínimo.");
    }
}