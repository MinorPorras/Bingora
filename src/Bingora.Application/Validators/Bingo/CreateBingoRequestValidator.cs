using Bingora.Application.DTOs.Bingo;
using FluentValidation;

namespace Bingora.Application.Validators.Bingo;

public class CreateOrUpdateBingoRequestValidator : AbstractValidator<CreateOrUpdateBingoRequestDto>
{
    public CreateOrUpdateBingoRequestValidator(ColumnRangeDtoValidator columnRangeValidator)
    {
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres");

        RuleFor(d => d.Description)
            .MaximumLength(500).WithMessage("La descripción puede ser de máximo 500 caracteres.");

        RuleFor(d => d.TotalCards)
            .GreaterThan(0).WithMessage("Los cartones totales deben de ser mayores a 0.");

        RuleFor(d => d.ColumnRanges)
            .NotEmpty().WithMessage("Los rangos de las columnas son necesarios.");

        RuleForEach(d => d.ColumnRanges).SetValidator(columnRangeValidator);
    }
}