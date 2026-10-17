using Bingora.Application.DTOs.User;
using FluentValidation;

namespace Bingora.Application.Validators.User;

public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequestDto>
{
    public UpdateEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("El nuevo correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato de correo electrónico es incorrecto")
            .MaximumLength(255).WithMessage("La contraseña no puede exceder los 255 caracteres");
    }
}