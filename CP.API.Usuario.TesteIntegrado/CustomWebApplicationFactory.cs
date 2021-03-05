using System;
using System.Linq;
using CP.Api.Usuario;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace CP.APi.Usuario.TesteUnitario
{
    #region snippet1
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(ApplicationContext));

                services.Remove(descriptor);

                services.AddDbContext<ApplicationContext>(options =>
                {
                    //options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UsuariosDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                    
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                    try
                    {
                        db.Database.EnsureCreated();
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                    
                    try
                    {
                        //using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                        //{
                        //    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
                        //    context.Database.EnsureCreated();
                        //}
                        // Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
#endregion
}
