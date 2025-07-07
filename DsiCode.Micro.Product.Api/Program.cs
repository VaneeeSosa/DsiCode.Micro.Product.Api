using System.Text;
using AutoMapper;
using DsiCode.Micro.Product.Api;
using DsiCode.Micro.Product.Api.Data;
using DsiCode.Micro.Product.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Registrar servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)),
        mysqlOptions =>
        {
            mysqlOptions.SchemaBehavior(
                MySqlSchemaBehavior.Translate,
                (originalName, defaultSchema) =>
                    originalName.Replace("dbo.", "")
            );
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    )
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);

builder.Services.AddCors();
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddEndpointsApiExplorer();

// 2️⃣ Construir la app
var app = builder.Build();

// 3️⃣ Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// --- Pipeline ---
// 4️⃣ Swagger en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroservicioProducto v1")
    );
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
