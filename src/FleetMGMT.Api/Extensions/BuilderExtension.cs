using System.Data;
using System.Reflection;
using FleetMGMT.Core;
using FleetMGMT.Core.Contexts.VehicleContext.Entities.Contracts;
using FleetMGMT.Infra.Contexts.DataContext;
using FleetMGMT.Infra.Contexts.VehicleContext.UseCases.CRUD;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;

namespace FleetMGMT.Api.Extensions;
public static class BuilderExtension
{
    // Adiciona as configurações padrão para inicialização
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        Configuration.Database.ConnectionString =
            builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }

    // Adiciona os serviços necessários
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.Services.AddSingleton<IDbConnection>(sp
         => new SqlConnection(Configuration.Database.ConnectionString));
        builder.Services.AddScoped<AppDbContext, AppDbContext>();
        builder.Services.AddTransient<IRepository, VehicleRepository>();
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FleetMGMT - API",
                Version = "v1",
                Description = "API com dados de locação de veículos"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}