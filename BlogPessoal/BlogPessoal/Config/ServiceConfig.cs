using System.Text;
using System.Text.Json.Serialization;
using BlogPessoal.Data;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace BlogPessoal.Config;

public static class ServiceConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddDatabaseConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mySqlConnection = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A connection string DefaultConnection nao foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

        return services;
    }

    public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
    {
        services.AddScoped<ITemaRepository, TemaRepository>();
        services.AddScoped<IPostagemRepository, PostagemRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpClient<IIaService, IaService>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        return services;
    }

    public static IServiceCollection AddAiConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiOptions>(configuration.GetSection("Ai"));

        return services;
    }

    public static IServiceCollection AddJwtConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("A chave Jwt:Secret nao foi configurada.");

        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];
        var jwtKey = Encoding.UTF8.GetBytes(jwtSecret);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "Digite: Bearer {token JWT retornado pelo login}. Exemplo: Bearer eyJ..."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference("Bearer", document, null)
                ] = new List<string>()
            });
        });

        return services;
    }
}
