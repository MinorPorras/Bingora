using Bingora.Api.Extensions;
using Bingora.Application.Validators.Auth;
using Bingora.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddCustomRateLimiting();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddProblemDetailsStandarization();
builder.Services.AddOpenApiDocumentation();

var app = builder.Build();

app.MapOpenApiDocumentationInDevelopment();
app.UseMiddlewarePipeline();
app.MapAllEndpoints();

app.Run();