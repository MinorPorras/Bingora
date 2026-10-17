using Bingora.Application.DTOs.User;
using Bingora.Domain.Enums;
using FluentValidation;

namespace Bingora.Application.Validators.User;

public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequestDto>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.NewRole)
            .IsInEnum().WithMessage("El rol proporcionado no es válido")
            .Must(role => role != Role.Admin)
            .WithMessage("No se puede asignar rol de administrador a través de este endpoint");
    }
}