using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.IO;

namespace DsiCode.Micro.Product.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 21)),
                mysqlOptions =>
                {
                    mysqlOptions.SchemaBehavior(
                        MySqlSchemaBehavior.Translate,
                        // Aquí recibiendo ambos parámetros:
                        (originalSchema, defaultSchema) =>
                            originalSchema.Replace("dbo.", "")
                    );
                }
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
