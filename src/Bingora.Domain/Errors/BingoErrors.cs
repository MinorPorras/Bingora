using Bingora.Domain.Common;

namespace Bingora.Domain.Errors;

public class BingoErrors
{
    public static readonly Error NotFound = new("Bingo.NotFound",
        "El bingo solicitado no existe.");

    public static readonly Error Inactive = new("Bingo.Inactive", "El bingo que se intenta usar está inactivo.");
}