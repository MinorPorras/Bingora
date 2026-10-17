using Bingora.Application.DTOs.User;
using FluentValidation;

namespace Bingora.Application.Validators.User;

public class UpdateUsernameRequestValidator : AbstractValidator<UpdateUsernameRequestDto>
{
    public UpdateUsernameRequestValidator()
    {
        RuleFor(x => x.NewUsername)
            .NotEmpty().WithMessage("El nuevo nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario debe contener menos de 50 caracteres.")
            .Matches("[^a-zA-Z0-9_]+$").WithMessage("Solo puede contener letras, números y guiones bajos.");
    }
}