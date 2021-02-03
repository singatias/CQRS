using Demo.AsyncProvider;
using Demo.Commands;
using Demo.DynamicQueries;
using Demo.Queries;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoweredSoft.CQRS;
using PoweredSoft.CQRS.Abstractions;
using PoweredSoft.CQRS.AspNetCore.Mvc;
using PoweredSoft.CQRS.DynamicQuery;
using PoweredSoft.CQRS.DynamicQuery.Abstractions;
using PoweredSoft.CQRS.DynamicQuery.AspNetCore;
using PoweredSoft.Data;
using PoweredSoft.Data.Core;
using PoweredSoft.DynamicQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
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
            AddQueries(services);
            AddDynamicQueries(services);
            AddCommands(services);

            services.AddTransient<IAsyncQueryableHandlerService, InMemoryQueryableHandler>();
            services.AddPoweredSoftDataServices();
            services.AddPoweredSoftDynamicQuery();

            services.AddPoweredSoftCQRS();
            services
                .AddControllers()
                .AddPoweredSoftQueries()
                .AddPoweredSoftCommands()
                .AddPoweredSoftDynamicQueries()
                .AddFluentValidation();

            services.AddSwaggerGen();
        }

        private void AddDynamicQueries(IServiceCollection services)
        {
            services.AddTransient<IQueryableProvider<Contact>, ContactQueryableProvider>();
            services.AddDynamicQuery<Contact, Contact>();
        }

        private void AddCommands(IServiceCollection services)
        {
            services.AddCommand<CreatePersonCommand, CreatePersonCommandHandler>();
            services.AddTransient<IValidator<CreatePersonCommand>, CreatePersonCommandValidator>();

            services.AddCommand<EchoCommand, string, EchoCommandHandler>();
            services.AddTransient<IValidator<EchoCommand>, EchoCommandValidator>();
        }

        private void AddQueries(IServiceCollection services)
        {
            services.AddQuery<PersonQuery, IQueryable<Person>, PersonQueryHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
