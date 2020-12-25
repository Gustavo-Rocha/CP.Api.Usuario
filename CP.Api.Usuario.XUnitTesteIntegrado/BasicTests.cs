using System.Threading.Tasks;
using CP.Api.Usuario;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CP.API.Usuario.TesteIntegrado
{
    #region snippet1
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory; 
        }

        [Theory]
       // [InlineData("/")]
       // [InlineData("/api")]
        [InlineData("/api/Usuario")]
       // [InlineData("/Get")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
    #endregion
}
