using Codibly.Services.Mailer.Application.Commands;
using Codibly.Services.Mailer.Infrastructure;
using Codibly.Services.Mailer.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Codibly.Services.Mailer.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var emailSenderOptions = new EmailSenderOptions();
            this.Configuration.GetSection("EmailSender").Bind(emailSenderOptions);
            services.AddInfrastructure(new MongoConnectionString(this.Configuration.GetConnectionString("MongoDb")),
                emailSenderOptions);

            services.AddMediatR(typeof(Startup), typeof(ICommand));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Codibly.Services.Mailer", Version = "v1",});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Codibly.Services.Mailer"); });

            // app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("", async context => await context.Response.WriteAsync("Mailer works!"));
                endpoints.MapControllers();
            });
        }
    }
}