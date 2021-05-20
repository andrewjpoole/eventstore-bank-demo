using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.Swagger;
using infrastructure;
using infrastructure.EventStore;
using infrastructure.StatisticsGatherer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using payment_scheme_domain.Services;

namespace payment_scheme_domain
{
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
            services.AddSingleton<IMediatrEndpointsProcessors, RequestProcessors>();
            services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
            services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
            services.AddHostedService(sp => (StatisticsQueuedHostedService)sp.GetService<IStatisticsQueuedHostedService>());
            services.AddTransient<ICatchupSubscription, CatchupSubscription>();
            services.AddTransient<IPersistentSubscriptionService, PersistentSubscriptionService>();
            services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
            services.AddSingleton<IEventPublisher, EventPublisher>();

            services.AddSingleton<IPaymentReceivedHostedService, PaymentReceivedHostedService>();
            services.AddHostedService(sp => (PaymentReceivedHostedService)sp.GetService<IPaymentReceivedHostedService>());
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
}
