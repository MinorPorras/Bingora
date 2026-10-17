using Bingora.Application.DTOs.User;
using Bingora.Domain.Enums;
using FluentValidation;

namespace Bingora.Application.Validators.User;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("La contraseña actual es obligatoria");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("La nueva contraseña es obligatoria")
            .MinimumLength(8).WithMessage("La contraseña debe de tener como mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe de contener al menos una mayúscula.")
            .Matches("[0-9]").WithMessage("Debe de contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Debe de contener al menos un caracter especial.")
            .NotEqual(x => x.CurrentPassword).WithMessage("La contraseña nueva debe de ser distinta a la anterior.");
    }
}