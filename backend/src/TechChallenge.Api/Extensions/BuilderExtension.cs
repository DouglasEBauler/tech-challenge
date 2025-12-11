using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Data;
using System.Reflection;
using System.Text;
using TechChallenge.Application.Behaviors;
using TechChallenge.Application.Validators;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;
using TechChallenge.Infra;
using TechChallenge.Infra.Persistence;
using TechChallenge.Infra.Persistence.CommandStores;
using TechChallenge.Infra.QueryStore;
using TechChallenge.JwtService;

namespace TechChallenge.Api.Extensions;

public static class BuilderExtension
{
    public static IServiceCollection AddApiAndSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TechChallenge API",
                Version = "v1",
                Description = "API for tech challenge"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Bearer {seu_token_jwt}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
            
            options.MapType<DateTime>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date",
                Example = new OpenApiString("1990-01-15")
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

            options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();

        services.AddSingleton(jwt);
        services.AddSingleton<IJwtSecurityService, JwtSecurityService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return context.Response.WriteAsync("Token is missing or invalid.");
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return context.Response.WriteAsync("Access denied.");
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IDbConnection>(_ =>
        {
            return new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IDapperQueryExecutor, DapperQueryExecutor>();
        services.AddScoped<IEmployeeCommandStore, EmployeeCommandStore>();
        services.AddScoped<IEmployeeQueryStore, EmployeeQueryStore>();
        services.AddScoped<IEmployeePhoneQueryStore, EmployeePhoneQueryStore>();

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        
        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeDomainService, EmployeeDomainService>();
        return services;
    }

    public static IServiceCollection AddMediatorAndPipelines(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(Domain.AssemblyReference).Assembly,
                typeof(Application.AssemblyReference).Assembly
            );

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(AssemblyValidatorReference).Assembly);

        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddCorsFrontend(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200", "https://localhost:4200");
            });
        });

        return services;
    }
}
