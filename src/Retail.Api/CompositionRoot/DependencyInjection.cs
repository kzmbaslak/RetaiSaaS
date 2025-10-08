using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Retail.Api.Filters.Tenant;
using Retail.Catalog.Application.Behaviors;
using Retail.Catalog.Application.Products.Commands.Create;
using Retail.Catalog.Domain.Repositories;
using Retail.Catalog.Infrastructure.Messaging;
using Retail.Catalog.Infrastructure.Messaging.Configuration;
using Retail.Catalog.Infrastructure.Messaging.RabbitMq;
using Retail.Catalog.Infrastructure.Messaging.Routing;
using Retail.Catalog.Infrastructure.Messaging.Serialization;
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
        // Options pattern
        service.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        service.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value);

        // Messaging 
        service.AddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();
        service.AddSingleton<IRoutingKeyResolver, ConventionRoutingKeyResolver>();
        service.AddSingleton<RabbitMqConnection>();
        service.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        service.AddSingleton<IEventSubscriber, RabbitMqSubscriber>();
        
        // Optional facade (tek servisle kullanmak istersen)
        service.AddSingleton<EventBus>();

        return service;
    }
    
}