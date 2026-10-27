using Bingora.Application.DTOs.Game;
using FluentValidation;

namespace Bingora.Application.Validators.Game;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequestDto>
{
    public CreateGameRequestValidator()
    {
        RuleFor(gp => gp.BingoId)
            .NotEmpty().WithMessage("El ID del bingo es obligatoria");

        RuleFor(gp => gp.Title)
            .NotEmpty().WithMessage("El título de la partida es obligatorio.")
            .MaximumLength(150).WithMessage("El título no puede exceder los 50 caracteres");

        RuleFor(gp => gp.AnimationDuration)
            .GreaterThanOrEqualTo(0).WithMessage("La duración de la animación no puede ser menor a 0.");
    }
}