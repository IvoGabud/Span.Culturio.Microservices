using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Span.Culturio.Users.Data;
using Span.Culturio.Users.Middleware;
using Span.Culturio.Users.Services;
using Span.Culturio.Users.Services.Interfaces;
using Span.Culturio.Users.Validators;
using System.Text;

var seqUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.WithProperty("Application", "Users Service")
    .WriteTo.Console()
    .WriteTo.File("logs/users-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq(seqUrl)
    .CreateLogger();

try
{
    Log.Information("Starting Users Service");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<UsersDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("UsersConnection")));

    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<GetUsersQueryDtoValidator>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<UsersDbContext>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Culturio Users API",
            Version = "v1",
            Description = "User management service (Admin only)"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        db.Database.Migrate();
    }

    app.UseMiddleware<GlobalExceptionHandler>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (builder.Configuration.GetValue<bool>("UseHttpsRedirection", true))
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Users Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
