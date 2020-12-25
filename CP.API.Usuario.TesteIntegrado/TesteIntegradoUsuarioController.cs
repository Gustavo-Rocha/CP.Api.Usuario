using CP.Api.Usuario;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CP.API.Usuario.TesteIntegrado
{
    public class TesteIntegradoUsuarioController
    {

        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        public ApplicationContext _context = new ApplicationContext();
        IList<Api.Usuario.Models.Usuario> ListaDeUsuarioPadrao;




        //public TesteIntegradoUsuarioController(ApplicationContext context)
        //{
        //    _context = context;
        //}

        [OneTimeSetUp]
        public async Task  PreparaTeste()
        {
           
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            await ExcluirUsuariosDoBancoAsync();
            

        }


        public async Task ExcluirUsuariosDoBancoAsync()
        {


           List<Api.Usuario.Models.Usuario> clientes= _context.Usuarios.ToList();

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
                        Cpf = "12345678923",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678922",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678924",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
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
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };

            var usuario2 = new List<Api.Usuario.Models.Usuario>()
            {
                  new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678923",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678922",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678924",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    }
            };

            foreach (Api.Usuario.Models.Usuario user in usuario2)
            {

                _context.Usuarios.Add(user);
                _context.SaveChanges();

            }

            //Act
            HttpResponseMessage post = await _client.PostAsync("/api/Usuario", new StringContent(
                JsonConvert.SerializeObject(usuario),Encoding.UTF8, "application/Json"));

            var resposta = await post.Content.ReadAsStringAsync();

            var conversao = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(resposta);

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
                Email = "neco@hotmail.com"
            };

            var usuario2 = new List<Api.Usuario.Models.Usuario>()
            {
                  new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678923",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678922",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678924",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    }
            };

            foreach (Api.Usuario.Models.Usuario user in usuario2)
            {

                _context.Usuarios.Add(user);
                _context.SaveChanges();

            }

            HttpResponseMessage delete = await _client.DeleteAsync($"/api/Usuario/{usuario.Cpf}");
            var response =await  delete.Content.ReadAsStringAsync();
            var conteudo = JsonConvert.DeserializeObject<Api.Usuario.Models.Usuario>(response);

            // buscar cpf deletado no banco 

            var cliente = _context.Usuarios.Find(usuario.Cpf);
            //Assert


            cliente.Should().BeNull();
            delete.Should().Be200Ok();


            // Assert.AreEqual(usuario.Cpf, cliente.Cpf);

           

        }

        [Test]
        public async Task PutDeveAtualizarUsuario()
        {
            //Arrange
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678900",
                Nome = "Gustavo Rocha",
                Celular = "123456789",
                Email = "neco@hotmail.com"
            };


            var usuario2 = new Api.Usuario.Models.Usuario
            {
                Cpf = usuario.Cpf,
                Nome = "Gustavo Rochass",
                Celular = "123456700",
                Email = "neco@gmail.com"
            };

            var usuario3 = new List<Api.Usuario.Models.Usuario>()
            {
                  new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678923",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678922",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    },new Api.Usuario.Models.Usuario
                    {
                        Cpf = "12345678924",
                        Nome = "Gustavo Rocha",
                        Celular ="952755705",
                        Email = "neco@hotmail.com"
                    }
            };

            foreach (Api.Usuario.Models.Usuario user in usuario3)
            {

                _context.Usuarios.Add(user);
                _context.SaveChanges();

            }



            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
       

            //Act
            HttpResponseMessage response = await _client.PutAsync("/api/Usuario", new StringContent(
                JsonConvert.SerializeObject(usuario2), Encoding.UTF8, "application/Json"));

             _context.Entry(usuario).Reload();
            var cliente = usuario;



            //Assert

            usuario2.Should().BeEquivalentTo(cliente);
            response.Should().Be204NoContent();

            //Assert.AreEqual(usuario.Celular, cliente.Celular);


            

        }




        [OneTimeTearDown]
        public async Task TearDown()
        {
            await ExcluirUsuariosDoBancoAsync();
            _client.Dispose();
            _factory.Dispose();
        }

    }
}
