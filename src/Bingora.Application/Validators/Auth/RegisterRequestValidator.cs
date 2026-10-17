using Bingora.Application.DTOs.Auth;
using FluentValidation;

namespace Bingora.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        //Reglas para username
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre de usuario debe de tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder los 50 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe de contener el menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("Debe de contener al menos una mayúscula.")
            .Matches("[0-9]").WithMessage("Debe de contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Debe de contener al menos un caracter especial.");
    }
}