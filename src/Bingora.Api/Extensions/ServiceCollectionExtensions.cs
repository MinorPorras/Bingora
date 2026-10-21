using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Bingora.Application.Interfaces;
using Bingora.Application.Services;
using Bingora.Infrastructure.Data;
using Bingora.Infrastructure.Repositories;
using Bingora.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Bingora.Api.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddJwtAuth(IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

                    RoleClaimType = ClaimTypes.Role
                };
            });
            services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public void AddCustomRateLimiting()
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(policyName: "AuthPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                        }));
                options.AddPolicy("GeneralPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 req/min es razonable
                            Window = TimeSpan.FromMinutes(1),
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Demasiadas solicitudes",
                        Detail = "Has excedido el límite de peticiones. Por favor, inténtalo de nuevo más tarde.",
                        Instance = context.HttpContext.Request.Path
                    };
                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, ct);
                };
            });
        }

        public void AddServices()
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        }

        public void AddRepositories()
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPatternRepository, PatternRepository>();
        }

        public void AddProblemDetailsStandarization()
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors.Select(x => x.ErrorMessage))
                        .ToArray();

                    var problemDetails = new ProblemDetails
                    {
                        Status = 400,
                        Title = "Error de validación",
                        Detail = string.Join("; ", errors),
                        Instance = context.HttpContext.Request.Path
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });
        }

        public void AddOpenApiDocumentation()
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??=
                        new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes["Bearer"] =
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = "Ingresa tu token JWT aquí"
                        };

                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });

                    return Task.CompletedTask;
                });
            });
        }
    }
}