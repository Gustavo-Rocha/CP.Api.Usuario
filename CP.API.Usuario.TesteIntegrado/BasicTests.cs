//using CP.Api.Usuario;
//using Microsoft.AspNetCore.Mvc.Testing;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xunit;
//using System.Threading.Tasks;

//namespace CP.API.Usuario.TesteIntegrado
//{
//    #region snippet1
//    public class BasicTests
//        : IClassFixture<WebApplicationFactory<Startup>>
//    {
//        private readonly WebApplicationFactory<Startup> _factory;

//        public BasicTests(WebApplicationFactory<Startup> factory)
//        {
//            _factory = factory; 
//        }

//        //[Xunit.Theory]
//        [Test]
//        [InlineData("/api/Usuario")]
//        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
//        {
//            // Arrange
//            var client = _factory.CreateClient();

//            // Act
//            var response = await client.GetAsync(url);

//            // Assert
//            response.EnsureSuccessStatusCode(); // Status Code 200-299
//            Assert.Equals("application/json; charset=utf-8",
//                response.Content.Headers.ContentType.ToString());
//        }
//    }
//    #endregion
//}
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using NUnit.Framework;

//namespace App.API.Tests.Integration
//{
//    [TestFixture]
//    public class SampleControllerTests
//    {
//        private APIWebApplicationFactory _factory;
//        private HttpClient _client;

//        [OneTimeSetUp]
//        public void GivenARequestToTheController()
//        {
//            _factory = new APIWebApplicationFactory();
//            _client = _factory.CreateClient();
//        }

//        [Test]
//        public async Task WhenSomeTextIsPosted_ThenTheResultIsOk()
//        {
//            var textContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Backpack for his applesauce"));
//            textContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

//            var result = await _client.PostAsync("/sample", textContent);
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//        }

//        [Test]
//        public async Task WhenNoTextIsPosted_ThenTheResultIsBadRequest()
//        {
//            var result = await _client.PostAsync("/sample", new StringContent(string.Empty));
//            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
//        }

//        [OneTimeTearDown]
//        public void TearDown()
//        {
//            _client.Dispose();
//            _factory.Dispose();
//        }
//    }
//}