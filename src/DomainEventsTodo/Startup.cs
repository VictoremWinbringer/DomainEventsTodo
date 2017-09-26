using Cassandra;
using DomainEventsTodo.Repositories.Abstract;
using DomainEventsTodo.Repositories.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DomainEventsTodo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var keySpace = Configuration["KeySpace"];
            var table = Configuration["Table"];
            var address = Configuration["Address"];

            using (Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1")
                .WithDefaultKeyspace("main")
                .Build())
            using (ISession session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists())
            {
                session.Execute(
                    $"create keyspace if not exists {keySpace} with replication ={{'class':'SimpleStrategy','replication_factor':3}};");

                session.Execute($"use {keySpace}");

                session.Execute(
                    $"create table if not exists {table}(Id uuid primary key, Description text, IsComplete boolean );");
            }

            services.AddScoped<ICluster>(s => Cluster.Builder().AddContactPoint(address)
                .Build());

            services.AddScoped<ISession>(s => s.GetService<ICluster>().Connect(keySpace));

            services.AddScoped<ITodoRepository, TodoRepository>();
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
        }
    }
}
