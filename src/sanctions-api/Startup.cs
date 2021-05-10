using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.EndpointRegistration;
using AJP.MediatrEndpoints.Swagger;
using infrastructure.EventStore;
using Microsoft.OpenApi.Models;
using sanctions_api.RequestHandlers;
using sanctions_api.RequestHandlers.AddRemoveName;
using sanctions_api.RequestHandlers.CheckName;
using sanctions_api.RequestHandlers.GetCurrentNames;
using sanctions_api.Services;

namespace sanctions_api
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
                    Title = "Sanctions Api",
                    Version = "v1",
                    Description = "Api for adding, removing, listing and checking sanctioned names, backed by an immutable stream in eventstore"
                });

                c.DocumentFilter<AddEndpointsDocumentFilter>();
            });

            services.AddMediatrEndpointsSwagger();

            services.AddMediatrEndpoints(typeof(Startup));
            services.AddLogging();
            services.AddSingleton<IEventStoreConnectionFactory, EventStoreConnectionFactory>();
            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<ICatchupSubscription, CatchupSubscription>();
            services.AddSingleton<ISanctionsCatchupHostedService, SanctionsCatchupHostedService>();
            services.AddHostedService(sp => (SanctionsCatchupHostedService)sp.GetService<ISanctionsCatchupHostedService>());
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sanctions API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGroupOfEndpointsForAPath("/sanctions", "Sanctions")
                    .WithPost<AddSanctionedNameRequest, SanctionedNameChangeResponse>("add")
                    .WithPost<RemoveSanctionedNameRequest, SanctionedNameChangeResponse>("remove")
                    .WithGet<GetCurrentSanctionedNamesRequest, CurrentSanctionedNamesResponse>("")
                    .WithPost<CheckNameRequest, CheckNameResponse>("check-name");
            });
        }
    }
}
