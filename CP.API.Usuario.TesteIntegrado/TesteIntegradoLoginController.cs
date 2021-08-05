using AutoMapper;
using CP.Api.Usuario;
using CP.Api.Usuario.Repository;
using CP.Api.Usuario.TokenJWT;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CP.API.Usuario.TesteIntegrado
{
    public class TesteIntegradoLoginController
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        public ApplicationContext _context = new ApplicationContext();
        IList<Api.Usuario.Models.Usuario> ListaDeUsuarioPadrao;
        private readonly UsuarioRepository usuarioRepository;
        private readonly TokenService tokenServices;
        private readonly IMapper mapper;

        public TesteIntegradoLoginController()
        {

        }

        [OneTimeSetUp]
        public async Task PreparaTeste()
        {
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public async Task iniciaFactoryCliente()
        {
            _client.Dispose();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public async Task Excluir()
        {
           // await ExcluirUsuariosDoBancoAsync();
        }

        public async Task ExcluirUsuariosDoBancoAsync()
        {
            List<Api.Usuario.Models.Usuario> clientes = _context.Usuarios.ToList();

            //HttpResponseMessage response = await _client.GetAsync("/api/Usuario");
            //string usuarios = await response.Content.ReadAsStringAsync();
            //var  conversao= JsonConvert.DeserializeObject<List<Api.Usuario.Models.Usuario>>(usuarios);

            //var okResult = retorno.Result as OkObjectResult;
            //var actualConfiguration = okResult.Value as Api.Usuario.Models.Usuario;
            foreach (Api.Usuario.Models.Usuario user in clientes)
            {
                //HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/api/Usuario/{user.Cpf}");
                //deleteResponse.EnsureSuccessStatusCode();

                _context.Usuarios.Remove(user);
                _context.SaveChanges();
            }
        }


        [Test]
        public async Task DeveEfetuarLoginComSucesso()
        {
            //Arrange
            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu",
                Email = "gustavooliveirarocha@hotmail.com"
            };

            //Act

            // var usuariosViewModel = mapper.Map<Api.Usuario.Models.Usuario>(usuario);
            //var token = new TokenService();
            //token.GenerateToken(usuario);

            //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage post = await _client.PostAsync($"/api/Login", new StringContent(
                JsonConvert.SerializeObject(login),Encoding.UTF8, "application/Json"));

             var resposta = await post.Content.ReadAsStringAsync();

            var conversao = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(resposta);

            //_context.Entry(usuario).Reload();

            //var cliente = _context.Usuarios.Find(usuario.Cpf);

            //Assert
            post.Should().Be200Ok();
           // usuario.Should().BeEquivalentTo(cliente);

            // Assert.AreEqual(usuario.Cpf, cliente.Cpf);
        }


        [Test]
        public async Task DeveEfetuarRecuperarSenhaComSucesso()
        {
            //Arrange
            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu",
                Email = "gustavooliveirarocha@hotmail.com"
            };
            var receiverEmail = "gustavooliveirarocha@hotmail.com";
                //Act

            // var usuariosViewModel = mapper.Map<Api.Usuario.Models.Usuario>(usuario);
            //var token = new TokenService();
            //token.GenerateToken(usuario);

            _client.DefaultRequestHeaders.Add("receiverEmail", receiverEmail);

            HttpResponseMessage post = await _client.PostAsync($"/api/Login/RecuperarSenha", new StringContent(
                JsonConvert.SerializeObject(login), Encoding.UTF8, "application/Json"));

            var resposta = await post.Content.ReadAsStringAsync();

            var conversao = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(resposta);

            //_context.Entry(usuario).Reload();

            //var cliente = _context.Usuarios.Find(usuario.Cpf);

            //Assert
            post.Should().Be200Ok();
            // usuario.Should().BeEquivalentTo(cliente);

            // Assert.AreEqual(usuario.Cpf, cliente.Cpf);
        }

    }
}
