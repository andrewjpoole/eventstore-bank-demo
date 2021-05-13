using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using events.Payments;
using infrastructure.EventStore;

namespace payment_scheme_simulator
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
            services.AddTransient<IEventPublisher, EventPublisher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/events/new-inbound-payment", async context =>
                {
                    var cancellationTokenSource = new CancellationTokenSource();

                    var publisher = context.RequestServices.GetService<IEventPublisher>();

                    var random = new Random();

                    var @event = new InboundPaymentReceived_v1
                    {
                        Amount = decimal.Parse($"{random.Next(1, 1_000_000)}.{random.Next(0, 99)}"),
                        PaymentReference = $"Simulated inbound payment {Guid.NewGuid().ToString().Substring(0,6)}",
                        ProcessingDate = DateTime.Now.Date,
                        Scheme = PaymentScheme.Bacs,
                        Type = PaymentType.Credit,
                        OriginatingSortCode = 209940,
                        OriginatingAccountNumber = random.Next(10000000, 99999999),
                        DestinationSortCode = 716151,
                        DestinationAccountNumber = random.Next(10000000, 99999999)
                    };

                    var result = await publisher.Publish(@event, "inbound-payments", cancellationTokenSource.Token);

                    await context.Response.WriteAsync($"{result}");
                });
            });
        }
    }
}
