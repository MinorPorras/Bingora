using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public class PatternErrors
{
    public static readonly Error NotFound = new("Pattern.NotFound",
        "El patrón solicitado no existe.");

    public static readonly Error NameAlreadyExists = new("Pattern.NameAlreadyExists",
        "Ya existe patrón registrado con ese nombre.");

    public static readonly Error Inactive = new("Pattern.Inactive", "El patrón que se intenta usar está inactivo.");
}