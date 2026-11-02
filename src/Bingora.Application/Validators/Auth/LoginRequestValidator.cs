using Bingora.Application.DTOs.Auth;
using FluentValidation;

namespace Bingora.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Input)
            .NotEmpty().WithMessage("El identificador es obligatorio.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("El password es obligatorio.")
            .MaximumLength(72).WithMessage("La contraseña no puede exceder más de  72 caracteres.");
    }
}