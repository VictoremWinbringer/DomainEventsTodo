using Cassandra;
using DomainEventsTodo.Dispatchers;
using DomainEventsTodo.Domain;
using DomainEventsTodo.Domain.Events;
using DomainEventsTodo.Handlers;
using DomainEventsTodo.SignalR;
using DomainEventsTodo.Validators;
using DomainEventsTodo.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace DomainEventsTodo
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddFluentValidation();

            services.AddSignalR();

            services.AddTransient<IValidator<TodoCreateVm>, TodoCreateValidator>();
            services.AddTransient<IValidator<TodoSearchVm>, TodoSearchValidator>();

            services.AddScoped<ICluster>(s => new Mock<ICluster>().Object);
            services.AddScoped<ISession>(s => new Mock<ISession>().Object);

            services.AddTransient<IDispatcher, DomainEventDispatcher>();

            services.AddTransient<IHandler<TodoComplete>, TodoCompleteService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            logger.AddConsole(Configuration);
            logger.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSignalR(b => b.MapHub<Notifier>("hub"));
        }
    }
}
