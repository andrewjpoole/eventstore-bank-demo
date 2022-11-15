using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using Domain.Interfaces;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using LedgerApp.Services;
using LedgerDomain.Behaviours;
using LedgerDomain.Events;
using LedgerDomain.ReadModel;
using LedgerDomain.RequestHandlers;
using Microsoft.OpenApi.Models;

namespace LedgerApp;

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
                Title = "Ledger Api",
                Version = "v1",
                Description = "Api for Ledgers and account balances"
            });

            c.DocumentFilter<AddEndpointsDocumentFilter>();
        });

        services.AddMediatrEndpointsSwagger();

        services.AddMediatrEndpoints(typeof(Startup));
        services.AddMediatrEndpoints(typeof(GetBalanceRequest));
        services.AddSingleton<IMediatrEndpointsProcessors, MediatrEndpointsProcessors>();
        services.AddLogging();
        services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
        services.AddTransient<IEventPublisher, EventPublisher>();
        services.AddTransient<ICatchupSubscription, CatchupSubscription>();
        services.AddTransient<IEventStreamReader, EventStreamReader>();

        services.AddSingleton<IEventDeserialiser, EventDeserialiser>();
        services.AddSingleton<IDeserialisationTypeMapper>(provider =>
        {
            var typeMapper = new DeserialisationTypeMapper();
            typeMapper.AddTypesFromAssembly(typeof(IEvent).Assembly);
            typeMapper.AddTypesFromAssembly(typeof(LedgerEntryPosted_v1).Assembly);
            return typeMapper;
        });
        
        services.AddTransient<ILedgerReadModel, LedgerReadModel>();
        services.AddSingleton<IPostLedgerEntryBehaviour, PostLedgerEntryBehaviour>();

        /*
         * LedgerService - open a transient ledgerReadmodel for the requested account for balance/balance check before posting etc.
         * LedgerSnapshotService - posts periodic snapshots?
         * LedgerCatchupSubscription - build SqlLite db of ledger entries?
         */
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ledger API V1");
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            /* Per Account
             * ===========
             * get balance for account
             * reserve funds?
             * un-reserve funds (if payment fails)?
             * post ledger entry (on successful payment)
             * Global
             * ======
             * get global position
             */
            endpoints.MapGroupOfEndpointsForAPath("/ledger", "Ledger")
                .WithGet<GetBalanceRequest, GetBalanceResponse>("balance")
                .WithPost<PostLedgerEntryRequest, PostLedgerEntryResponse>("");
        });
    }
}

public class MediatrEndpointsProcessors : IMediatrEndpointsProcessors
{
    public Action<HttpContext, ILogger> PreProcess { get; set; }
    public Action<HttpContext, TimeSpan, ILogger> PostProcess { get; set; }
    public Action<Exception, HttpContext, ILogger> ErrorProcess { get; set; }

    public MediatrEndpointsProcessors()
    {
        PreProcess = (context, logger) => { };
        PostProcess = (context, span, arg3) => { };
        ErrorProcess = (exception, context, arg3) =>
        {
            // log error
        };
    }
}