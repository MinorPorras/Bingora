namespace Bingora.Api.Helpers;

public static class ProblemResults
{
    private static IResult Create(string title, int statusCode, string? detail = null)
        => Results.Problem(title: title, statusCode: statusCode, detail: detail);

    public static IResult NotFound(string title = "Recurso no encontrado", string? detail = null)
        => Create(title, StatusCodes.Status404NotFound, detail);

    public static IResult Unauthorized(string title = "Usuario no autenticado", string? detail = null)
        => Create(title, StatusCodes.Status401Unauthorized, detail);

    public static IResult Conflict(string title = "Conflicto con el estado actual", string? detail = null)
        => Create(title, StatusCodes.Status409Conflict, detail);

    public static IResult Forbidden(string title = "Acceso denegado, , permisos insuficientes", string? detail = null)
        => Create(title, StatusCodes.Status403Forbidden, detail);

    public static IResult BadRequest(string title = "Solicitud incorrecta", string? detail = null)
        => Create(title, StatusCodes.Status400BadRequest, detail);

    public static IResult Validation(IDictionary<string, string[]> errors,
        string title = "Errores de validación en la solicitud", string? detail = null)
        => Results.ValidationProblem(errors,
            title: title,
            statusCode: StatusCodes.Status400BadRequest,
            detail: detail);
}