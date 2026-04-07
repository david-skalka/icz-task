using System.Text;
using IczTask.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IczTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<TaskSettings>(
            builder.Configuration.GetSection("TaskSettings")
        );


// 🔹 Controllers
        builder.Services.AddControllers();

// 🔹 Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IczTask API", Version = "v1" });

            // 🔹 Přidání podpory Bearer tokenu
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
                    []
                }
            });
        });

        
        if (!builder.Environment.IsEnvironment("Integration"))
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("TaskDb"));
                }
            );

        var jwtSection = builder.Configuration.GetSection("Jwt");
        builder.Services.Configure<JwtOptions>(jwtSection);
        var jwtOptions = jwtSection.Get<JwtOptions>()!;

        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        builder.Services.AddAuthorization();


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost", x =>
            {
                x.WithOrigins("https://localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });


        var app = builder.Build();

        app.UseCors("AllowLocalhost");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapFallbackToFile("/{*path:nonfile}", "index.html");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        using (var scope = app.Services.CreateScope())
        {
            if (!scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().IsEnvironment("Integration"))
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.SaveChanges();
        }


        app.Run();
    }
}