using System.Collections.Generic;
using AccountsApp.Services;
using AccountsDomain.Events;
using AccountsDomain.RequestHandlers.Accounts;
using AccountsDomain.ServiceInterfaces;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Infrastructure.StatisticsGatherer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AccountsApp;

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
                Title = "Accounts Api",
                Version = "v1",
                Description = "Api for adding, modifying, and listing accounts and transactions, backed by immutable streams in eventstore"
            });

            c.DocumentFilter<AddEndpointsDocumentFilter>();
        });

        services.AddMediatrEndpointsSwagger();
        services.AddMediatrEndpoints(typeof(AccountOpenedEvent_v1));

        services.AddLogging();
        services.AddSingleton<IMediatrEndpointsProcessors, DefaultRequestProcessors>();
        services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
        services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
        services.AddHostedService(sp => (StatisticsQueuedHostedService)sp.GetService<IStatisticsQueuedHostedService>());
        
        services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
        services.AddTransient<IEventPublisher, EventPublisher>();
        services.AddTransient<ICatchupSubscription, CatchupSubscription>();
        services.AddTransient<IEventStreamReader, EventStreamReader>();

        services.AddSingleton<IEventDeserialiser, EventDeserialiser>();
        services.AddSingleton<IDeserialisationTypeMapper>(provider =>
        {
            var typeMapper = new DeserialisationTypeMapper();
            typeMapper.AddTypesFromAssembly(typeof(IEvent).Assembly);
            typeMapper.AddTypesFromAssembly(typeof(AccountOpenedEvent_v1).Assembly);
            return typeMapper;
        });

        services.AddSingleton<IAccountsCatchupHostedService, AccountsCatchupHostedService>();
        services.AddHostedService(sp => (AccountsCatchupHostedService)sp.GetService<IAccountsCatchupHostedService>());
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddScoped<IEndpointContextAccessor, EndpointContextAccessor>();
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts API V1");
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
                .WithGet<GetAccountsRequest, IEnumerable<AccountSummary>>("/", "Gets Accounts with various filter options")
                .WithGet<GetAccountByIdRequest, AccountSummary>("/{Id}", "Get a single account by Id")
                .WithPost<CreateAccountRequest, CreateAccountResponse>("/", "Create a new account", StatusCodes.Status202Accepted)
                .WithPut<UpdateAccountStatusRequest, UpdateAccountStatusResponse>("/{Id}", "Update the status of an account", StatusCodes.Status202Accepted, 
                    ParameterDictionaryBuilder.NewDictionary()
                        .AddEnumParam("Status", typeof(AccountStatus), ParameterDictionaryBuilder.In.Query, true));

            endpoints.MapGet("/Stats", async context =>
            {
                var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                await context.Response.WriteAsJsonAsync(statsGatherer?.GetStats());
            });
        });
    }
}