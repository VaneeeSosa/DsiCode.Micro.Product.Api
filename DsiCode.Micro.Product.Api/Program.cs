using System.Collections.Generic;
using System.Text;
using AutoMapper;
using DsiCode.Micro.Product.Api;
using DsiCode.Micro.Product.Api.Data;
using DsiCode.Micro.Product.Api.Extensions; // ← Aquí van tus métodos de extensión
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Configurar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) AutoMapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 3) CORS (si lo necesitas)
builder.Services.AddCors();

// 4) Controllers + JsonOptions (para ignorar ciclos)
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// 5) Registrar tu autenticación JWT (extensión definida en AuthenticationExtensions.cs)
builder.Services.AddJwtAuthentication(builder.Configuration);

// 6) Registrar Swagger (extensión definida en AuthenticationExtensions.cs)
builder.Services.AddSwaggerConfiguration();

// 7) Registrar la exploración de endpoints (si usas Minimal APIs u OpenAPI)
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// --- Pipeline ---

// 8) Swagger en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroservicioProducto v1");
    });
}

app.UseHttpsRedirection();

// 9) Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
