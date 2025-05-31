using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DsiCode.Micro.Product.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Configura la autenticación JWT basado en los valores de configuración en "ApiSettings:JwtOptions".
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) Obtener la clave secreta desde appsettings.json:
            //    "ApiSettings": {
            //      "JwtOptions": {
            //        "Secret": "MiClaveMuySecreta12345",
            //        "Issuer": "MiIssuer",
            //        "Audience": "MiAudience"
            //      }
            //    }
            var secretKey = configuration["ApiSettings:JwtOptions:Secret"];
            var issuer = configuration["ApiSettings:JwtOptions:Issuer"];
            var audience = configuration["ApiSettings:JwtOptions:Audience"];

            // 2) Convertir la clave secreta a byte[] y crear SymmetricSecurityKey
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            // 3) Configurar los servicios de autenticación
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateLifetime = true,
                        ClockSkew = System.TimeSpan.FromMinutes(2) // opcional: ajuste de tolerancia
                    };
                });

            // 4) Configurar la política de autorización por defecto (requiere usuario autenticado en todas las rutas)
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }

        /// <summary>
        /// Configura Swagger (Swashbuckle) y define el esquema de seguridad "Bearer" para JWT.
        /// </summary>
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MicroservicioProducto",
                    Version = "v1"
                });

                // 1) Definición del esquema de seguridad "Bearer"
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese 'Bearer' [Espacio] y luego su token"
                });

                // 2) Requisito global de usar ese esquema en todas las rutas
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        // ← Abrimos el par (clave, valor) para el diccionario
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            return services;
        }
    }
}
