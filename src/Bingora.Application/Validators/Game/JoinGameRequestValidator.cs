using Bingora.Application.DTOs.Game;
using FluentValidation;

namespace Bingora.Application.Validators.Game;

public class JoinGameRequestValidator : AbstractValidator<JoinGameRequestDto>
{
    public JoinGameRequestValidator()
    {
        RuleFor(d => d.PlayerName)
            .NotEmpty().WithMessage("El nombre del jugador es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del jugador no puede ser mayor a 100 caracteres.");

        RuleFor(d => d.AccessPassword).MaximumLength(10)
            .WithMessage("La contraseña no puede exceder los 10 caracteres.");

        RuleFor(d => d.GameCode)
            .NotEmpty().WithMessage("El código del juego es obligatorio.")
            .MaximumLength(10).WithMessage("El código no puede exceder más de 10 caracteres.");
    }
}