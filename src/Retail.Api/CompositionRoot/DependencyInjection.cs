using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Retail.Api.Filters.Tenant;
using Retail.Catalog.Application.Behaviors;
using Retail.Catalog.Application.Products.Commands.Create;
using Retail.Catalog.Domain.Repositories;
using Retail.Catalog.Infrastructure.Messaging;
using Retail.Catalog.Infrastructure.Persistence;
using Retail.Catalog.Infrastructure.Repositories;
using Retail.Catalog.Infrastructure.Service;
using Retail.Shared.Abstractions.MultiTenancy;
using Retail.Shared.Abstractions.Time;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Api.ComositionRoot;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddControllers();
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();
        service.AddCors(options => //CORS (Cross-Origin Resource Sharing)
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return service;
    }

    public static IServiceCollection AddApplication(this IServiceCollection service)
    {
        service.AddMediatR(typeof(CreateProductCommand).Assembly);
        service.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);
        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return service;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddHttpContextAccessor();
        service.AddScoped<ITenantProvider, HttpTenantProvider>();

        service.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("pg"));
            //options.UseSnakeCaseNamingConvention();
        });

        service.AddScoped<IProductRepository, ProductRepository>();
        service.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // RabbitMQ EventBus
        var mq = configuration.GetSection("RabbitMq");
        service.AddSingleton<IEventBus>(_ => new RabbitMQEventBus(
            host: mq.GetValue<string>("host")!,
            port: mq.GetValue<int>("port"),
            user: mq.GetValue<string>("user")!,
            pass: mq.GetValue<string>("pass")!,
            exchange: mq.GetValue<string>("exchange")!
        ));

        return service;
    }
    
}