using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using Infrastructure.EventStore;
using Microsoft.OpenApi.Models;
using payment_scheme_simulator.RequestHandlers;
using payment_scheme_simulator.Services;

namespace payment_scheme_simulator;

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
                Title = "Scheme Simulator Api",
                Version = "v1",
                Description = "Api for simulating inbound payments"
            });

            c.DocumentFilter<AddEndpointsDocumentFilter>();
        });

        services.AddMediatrEndpointsSwagger();

        services.AddMediatrEndpoints(typeof(Startup));

        services.AddSingleton<IEventStoreClientFactory, EventStoreClientFactory>();
        services.AddTransient<IEventPublisher, EventPublisher>();
        services.AddSingleton<IRandomInboundPaymentReceivedGenerator, RandomInboundPaymentReceivedGenerator>();
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scheme Simulator API V1");
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGroupOfEndpointsForAPath("/api/v1/inbound-payments", "Inbound Payments", "methods to simulate inbound payments")
                .WithPost<SimulateRandomInboundPaymentRequest, SimulatedInboundPaymentResponse>("random",
                    "Simulates an inbound payment of a random amount, from a random sortcode and account number, to a random owned account.")
                .WithPost<SimulateInboundPaymentRequest, SimulatedInboundPaymentResponse>("manual",
                    "Simulates an inbound payment of a specified amount, from a specified sortcode and account number, to a specified owned account. Any fields left blank will be randomised");
            // add start and stop methods and a background service which will randomly spit out random payments according to min and max params etc

        });
    }
}