using AccountsDomain.Events;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.Swagger;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Infrastructure.StatisticsGatherer;
using LedgerClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentReadModel;
using PaymentSchemeApp.Services;
using PaymentSchemeDomain.Events;
using SanctionsClient;

namespace PaymentSchemeApp;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(); // only needed for swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Payment Scheme A Domain Api",
                Version = "v1",
                Description = "implementation of Payment Scheme A"
            });

            c.DocumentFilter<AddEndpointsDocumentFilter>();
        });

        services.AddMediatrEndpointsSwagger();

        services.AddMediatrEndpoints(typeof(Startup));

        services.AddLogging();
        services.AddSingleton<IMediatrEndpointsProcessors, DefaultRequestProcessors>();
        services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
        services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
        services.AddHostedService(sp => sp.GetService<IStatisticsQueuedHostedService>() as StatisticsQueuedHostedService);
        services.AddTransient<ICatchupSubscription, CatchupSubscription>();
        services.AddTransient<IEventStreamReader, EventStreamReader>();
        services.AddTransient<IPersistentSubscriptionService, PersistentSubscriptionService>();
        services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
        services.AddSingleton<IEventPublisher, EventPublisher>();
        services.AddSingleton<ISanctionsApiClient, SanctionsApiClient>();
        services.AddSingleton<IInboundPaymentReadModelFactory, InboundPaymentReadModelFactory>();
        services.AddSingleton<IAccountDetailsReadModelFactory, AccountDetailsReadModelFactory>();
        services.AddTransient<IInboundPaymentReadModel, InboundPaymentReadModel>();
        services.AddTransient<IAccountDetailsReadModel, AccountDetailsReadModel>();
        services.AddSingleton<ILedgerApiClient, LedgerApiClient>();

        services.AddSingleton<IEventDeserialiser, EventDeserialiser>();
        services.AddSingleton<IDeserialisationTypeMapper>(provider =>
        {
            var typeMapper = new DeserialisationTypeMapper();
            typeMapper.AddTypesFromAssembly(typeof(IEvent).Assembly);
            typeMapper.AddTypesFromAssembly(typeof(InboundPaymentReceived_v1).Assembly);
            typeMapper.AddTypesFromAssembly(typeof(AccountOpenedEvent_v1).Assembly);
            return typeMapper;
        });

        services.AddSingleton<IPaymentValidaterHostedService, PaymentValidaterHostedService>();
        services.AddHostedService(sp => sp.GetService<IPaymentValidaterHostedService>() as PaymentValidaterHostedService);

        services.AddSingleton<IPaymentSanctionsCheckerHostedService, PaymentSanctionsCheckerHostedService>();
        services.AddHostedService(sp => sp.GetService<IPaymentSanctionsCheckerHostedService>() as PaymentSanctionsCheckerHostedService);
        
        services.AddSingleton<IPaymentReleasedHostedService, PaymentReleasedHostedService>();
        services.AddHostedService(sp => sp.GetService<IPaymentReleasedHostedService>() as PaymentReleasedHostedService);

        services.AddSingleton<IPaymentAccountStatusCheckerHostedService, PaymentAccountStatusCheckerHostedService>();
        services.AddHostedService(sp => sp.GetService<IPaymentAccountStatusCheckerHostedService>() as PaymentAccountStatusCheckerHostedService);

        services.AddSingleton<IPaymentAccountTransactionCreationHostedService, PaymentAccountTransactionCreationHostedService>();
        services.AddHostedService(sp => sp.GetService<IPaymentAccountTransactionCreationHostedService>() as PaymentAccountTransactionCreationHostedService);
        
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Scheme A API V1");
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            // payment scheme endpoints go here...

            endpoints.MapGet("/Stats", async context =>
            {
                var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                await context.Response.WriteAsJsonAsync(statsGatherer?.GetStats());
            });
        });
    }
}