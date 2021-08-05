using AutoMapper;
using CP.Api.Usuario.Criptografia;
using CP.Api.Usuario.EmailConfiguration;
using CP.Api.Usuario.EmailService;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using CP.Api.Usuario.TokenJWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CP.Api.Usuario
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
            services.AddControllers();

            var connectionString = Configuration.GetConnectionString("Default");

            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<ICriptografar, Criptografar>();
            services.AddTransient<IHash256,Hash256>();
            services.AddSingleton<HashAlgorithm>(SHA256.Create());
            
            var notificationMetadata = Configuration.GetSection("NotificationMetadata").Get<NotificationMetadata>();
            services.AddSingleton(notificationMetadata);
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddTransient<ITokenService, TokenService>();
            // services.AddTransient<HashAlgorithm>(new HashAlgorithm());

            // Configurando o serviço de documentação do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Crud de Usuários",
                        Version = "v1",
                        Description = "API para CRUD de usuários",
                    });

                string caminhoAplicacao =
                    PlatformServices.Default.Application.ApplicationBasePath;
                string nomeAplicacao =
                    PlatformServices.Default.Application.ApplicationName;
                string caminhoXmlDoc =
                    Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");

                //c.IncludeXmlComments(caminhoXmlDoc);
            });

            //services.AddHealthChecks()
            //    .AddCheck<HealthCheck>(
            //        "health_check",
            //         failureStatus: HealthStatus.Degraded,
            //         tags: new[] { "example" });

            services.AddHealthChecks()
                .AddSqlServer(connectionString);

            //services.AddHealthChecks()
            //  .AddDbContextCheck<ApplicationContext>();

            AutoMapperConfig(services);

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });  
        }

        private void AutoMapperConfig(IServiceCollection services)
        {
            var mapperConfiguration = new MapperConfiguration(config=>
            {
                config.CreateMap<UsuarioViewModel, Models.Usuario>(); 
            });

            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
            //    context.Database.EnsureCreated();
            //}

            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/health");
            });

            // Ativando middlewares para uso do Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "API de Usuários");
            });  
        }
    }
}
