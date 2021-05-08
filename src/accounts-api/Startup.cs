using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using accounts_api.Services;
using AJP.MediatrEndpoints;
using AJP.MediatrEndpoints.EndpointRegistration;
using infrastructure.StatisticsGatherer;

namespace accounts_api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatrEndpoints(typeof(Startup));

            services.AddLogging();
            services.AddSingleton<IMediatrEndpointsProcessors, RequestProcessors>();
            services.AddSingleton<IStatisticsTaskQueue, StatisticsTaskQueue>();
            services.AddSingleton<IStatisticsQueuedHostedService, StatisticsQueuedHostedService>();
            services.AddHostedService(sp => (StatisticsQueuedHostedService)sp.GetService<IStatisticsQueuedHostedService>());
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

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World! welcome to the accounts-api");
                });

                endpoints.MapGroupOfEndpointsForAPath("/api/v1/accounts", "Accounts", "everything to do with accounts")
                    .WithGet<GetAccountsRequest, IEnumerable<AccountDetails>>("/", "Gets Accounts with various filter options")
                    .WithGet<GetAccountByIdRequest, AccountDetails>("/{Id}", "Get a single account by Id")
                    .WithPost<CreateAccountRequest, CreateAccountResponse>("/", "Create a new account", StatusCodes.Status201Created)
                    .WithDelete<DeleteAccountByIdRequest, AccountDeletedResponse>("/{Id}", "Delete an account by Id", StatusCodes.Status204NoContent)
                    .WithPut<UpdateAccountStatusRequest, AccountDetails>("/{Id}");

                endpoints.MapGet("/Stats", async context =>
                {
                    var statsGatherer = context.RequestServices.GetService<IStatisticsQueuedHostedService>();
                    await context.Response.WriteAsJsonAsync(statsGatherer?.GetStats());
                });
            });
        }
    }
}
