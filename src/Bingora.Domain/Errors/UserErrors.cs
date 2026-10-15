using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = new("User.NotFound",
        "El usuario con el correo electrónico proporcionado no existe");

    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "El correo electrónico o la contraseña son incorrectos");

    public static readonly Error EmailAlreadyExists = new("User.EmailAlreadyExists",
        "Ya existe una cuenta registrada con este correo electrónico");

    public static readonly Error InvalidOrExpiredRefreshToken = new("User.InvalidOrExpiredRefreshToken",
        "El token de actualización es inválido o ha expirado");
}