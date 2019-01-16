using Aguacongas.RedisQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Net.Http;

namespace Aguacongas.RedisQueue.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var version = GetType().Assembly.GetName().Version;

            Version = $"{version.Major}.{version.Minor}";
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; }

        /// <summary>
        /// Configures the services.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSwaggerGen(c =>
                {
                    var info = new Info
                    {
                        Version = Version,
                        Contact = new Contact
                        {
                            Email = "aguacongas@gmail.com",
                            Name = "Olivier Lefebvre",
                            Url = "https://github.com/aguacongas/DymamicAuthProviders"
                        },
                        License = new License
                        {
                            Name = "MIT",
                            Url = "https://github.com/aguacongas/DymamicAuthProviders/blob/master/LICENSE"
                        },
                        Title = "Aguacongas.RedisQueue API"
                    };

                    c.SwaggerDoc(info.Version, info);

                    // Set the comments path for the Swagger JSON and UI.
                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                    var xmlPath = Path.Combine(basePath, "Aguacongas.RedisQueue.Web.xml");
                    c.IncludeXmlComments(xmlPath);
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                })
                .AddAuthorization(options =>
                {
                    options.AddPolicy("RedisQueues", policy =>
                    {
                        policy.Requirements.Add(new RedisQueueRequirement());
                    });
                })
                .AddRedisQueue("localhost:6379")
                .AddHttpClient()
                .AddTransient(provider => provider.GetRequiredService<IHttpClientFactory>().CreateClient())
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// Configures the specified application.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/{Version}/swagger.json", "Aguacongas.RedisQueue");
                })
                .UseSignalR(options =>
                {
                    options.MapHub<QueueHub>("/queues");
                })
                .UseMvc();
        }
    }
}
