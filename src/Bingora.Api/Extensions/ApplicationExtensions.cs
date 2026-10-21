using Bingora.Api.Endpoints;
using Scalar.AspNetCore;

namespace Bingora.Api.Extensions;

public static class ApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapOpenApiDocumentationInDevelopment()
        {
            if (!app.Environment.IsDevelopment()) return;
            app.MapOpenApi().AllowAnonymous();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Bingora - Documentación (Scalar)");
                options.WithTheme(ScalarTheme.DeepSpace);
                options.WithOpenApiRoutePattern("/openapi/v1.json");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            }).AllowAnonymous();
        }

        public void UseMiddlewarePipeline()
        {
            app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public void MapAllEndpoints()
        {
            app.MapAuthEndpoints();
            app.MapUserManagementEndpoints();
            app.MapProfileEndpoints();
            app.MapPatternEndpoints();
        }
    }
}