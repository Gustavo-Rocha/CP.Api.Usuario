using CP.Api.Usuario;
using CP.Api.Usuario.Criptografia;
using CP.Api.Usuario.TokenJWT;
using CP.APi.Usuario.TesteUnitario;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CP.API.Usuario.TesteIntegrado
{
    class TesteWebApplicationFactory
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        public ApplicationContext _context = new ApplicationContext();
        IList<Api.Usuario.Models.Usuario> ListaDeUsuarioPadrao;

        //public TesteIntegradoUsuarioController(ApplicationContext context)
        //{
        //    _context = context;
        //}

        [OneTimeSetUp]
        public async Task PreparaTeste()
        {

            _factory = new CustomWebApplicationFactory<Startup>();
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
            await ExcluirUsuariosDoBancoAsync();

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
        public async Task DeveObterTodosUsuarios()
        {
            //Arrange
            var usuario = new List<Api.Usuario.Models.Usuario>()
            {
                  new Api.Usuario.Models.Usuario
                    {
                        Cpf = new Random().Next(0, 999999999).ToString("00000000000"),
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = new Random().Next(0, 999999999).ToString("00000000000"),
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = new Random().Next(0, 999999999).ToString("00000000000"),
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    }
            };
            usuario.ForEach(u => _context.Usuarios.Add(u));
            _context.SaveChanges();

            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/Usuario");
            string usuarios = await response.Content.ReadAsStringAsync();
            var conversao = JsonConvert.DeserializeObject<List<Api.Usuario.Models.Usuario>>(usuarios);

            // Assert

            conversao.Should().BeEquivalentTo(usuario);
            conversao.Should().HaveCount(usuario.Count);

        }

        [Test]
        public async Task DeveEfetuarPostComSucessoAsync()
        {
            //Arrange
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "01020304055",
                Nome = "Gustavo Rocha",
                Celular = "11952755705",
                Email = "neco@hotmail.com",
                Senha = "12345678"
            };

            var usuario2 = new List<Api.Usuario.Models.Usuario>()
            { 
                new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678923",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678922",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678924",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com",
                        Senha= "12345678"
                    }
            };

            Hash256 hash = new Hash256(SHA256.Create());

            foreach (Api.Usuario.Models.Usuario user in usuario2)
            {

                user.Senha = hash.CriptografarSenha(usuario.Senha);
                _context.Usuarios.Add(user);
                _context.SaveChanges();

            }

            //Act
            HttpResponseMessage post = await _client.PostAsync("/api/Usuario", new StringContent(
                JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/Json"));

            var resposta = await post.Content.ReadAsStringAsync();

            var conversao = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(resposta);
            _context.Entry(usuario).Reload();

            var cliente = _context.Usuarios.Find(usuario.Cpf);

            //Assert
            post.Should().Be201Created();
            usuario.Should().BeEquivalentTo(cliente);

            // Assert.AreEqual(usuario.Cpf, cliente.Cpf);
        }

        [Test]
        public async Task DeveEfetuarDeleteComSucesso()
        {
            //Arrange
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678923",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com",
                Senha = "12345678"
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var token = new TokenService();
            var autorizacao  = token.GenerateToken(usuario);

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + autorizacao);

            HttpResponseMessage delete = await _client.DeleteAsync($"/api/Usuario/{usuario.Cpf}");
            var response = await delete.Content.ReadAsStringAsync();
            var conteudo = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(response);

            // buscar cpf deletado no banco 
            _context.Entry(usuario).Reload();
            var cliente = _context.Usuarios.Find(usuario.Cpf);
            //Assert

            delete.Should().Be200Ok();
            cliente.Should().BeNull();

            // Assert.AreEqual(usuario.Cpf, cliente.Cpf);
        }

        [Test]
        public async Task PutDeveAtualizarUsuario()
        {
            //Arrange
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "11952755705",
                Email = "neco@hotmail.com",
                Senha = "240497gu"
            };

            var usuario2 = new Api.Usuario.Models.Usuario
            {
                Cpf = usuario.Cpf,
                Nome = "Gustavo Rochass",
                Celular = "11952755705",
                Email = "neco@gmail.com",
                Senha = "240497gu"
            };

            var token = new TokenService();
            var autorizacao = token.GenerateToken(usuario);
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + autorizacao);

            HttpResponseMessage responsePost = await _client.PostAsync("/api/Usuario", new StringContent(
                 JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/Json"));

            //Act
            HttpResponseMessage response = await _client.PutAsync("/api/Usuario", new StringContent(
                JsonConvert.SerializeObject(usuario2), Encoding.UTF8, "application/Json"));

            _context.Entry(usuario).Reload();
            var cliente = usuario;

            //Assert
            //usuario2.Should().BeEquivalentTo(cliente);
            response.Should().Be204NoContent();
            //Assert.AreEqual(usuario.Celular, cliente.Celular);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            // await ExcluirUsuariosDoBancoAsync();
            _client.Dispose();
            _factory.Dispose();
        }

    }

}