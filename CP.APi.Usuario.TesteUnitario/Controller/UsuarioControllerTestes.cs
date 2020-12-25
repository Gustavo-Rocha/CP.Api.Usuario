using CP.Api.Usuario.Controllers;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CP.APi.Usuario.TesteUnitario
{
    public class UsuarioControllerTestes
    {
        [Test]
        public void DeveObterTodosUsuarios()
        {

            var usuarios = new List<Api.Usuario.Models.Usuario>()
                {
                    new Api.Usuario.Models.Usuario()
                    {
                        Cpf = "123456+75"
                    },
                        new Api.Usuario.Models.Usuario()
                    {
                        Cpf = "4588565"
                    },
                        new Api.Usuario.Models.Usuario()
                    {
                        Cpf = "7854654"
                    },
                        new Api.Usuario.Models.Usuario()
                    {
                        Cpf = "5454546"
                    },
                        new Api.Usuario.Models.Usuario()
                    {
                        Cpf = "989898"
                    }
                    };

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Returns(usuarios);
            var controller2 = new UsuarioController( mockUsuarioRepository.Object);

            var retorno =  controller2.Get();

            var result = retorno.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            //Assert.AreEqual(usuarios, retorno);

        }

        [Test]
        public void DeveRetornarListaVazia()
        {

            var listaVazia = new List<Api.Usuario.Models.Usuario>();

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Returns(listaVazia);
            var controller2 = new UsuarioController( mockUsuarioRepository.Object);

            var resposta =  controller2.Get();

            var conflict = resposta.Result as OkObjectResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(200, conflict.StatusCode);

            //Assert.IsEmpty(resposta.ToString());
        }

        [Test]
        public void DeveRetornarUmaExcecao()
        {
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Throws<NullReferenceException>();
            var controller2 = new UsuarioController( mockUsuarioRepository.Object);

            Assert.Throws<NullReferenceException>(() => controller2.Get());

        }

        [Test]
        public void PostDeveEfetuarCadastroDoUsuario()
        {
            var usuario = new Api.Usuario.Models.Usuario();

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(usuario)).Verifiable();

            var usuarioRepository = mockUsuarioRepository.Object;
            var context = new Api.Usuario.ApplicationContext();
            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Post(usuario);


            var conflict = retorno.Result as CreatedAtActionResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(201, conflict.StatusCode);

            Mock.Get(usuarioRepository).Verify(v => v.Cadastrar(It.IsAny<Api.Usuario.Models.Usuario>()), Times.Once);
            //Mock.Get(usuarioRepository).Verify(u => u.Consultar(), Times.Exactly(1));
            //Mock.Get(context).Verify(c => c.Usuarios.Any(c => c.Email == "Teste@teste"));
            //passa a instrução  verificar se  chamando esse metodo cadastra com  a variavel


        }
        [Test]
        public void PostDeveRetornarExcecao()
        {
            Api.Usuario.Models.Usuario usuarios = null;
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(null)).Throws<ArgumentException>();

            //o setup é como se fosse o cenário...
            var controller = new UsuarioController( mockUsuarioRepository.Object);

            Assert.Throws<ArgumentException>(() => controller.Post(usuarios));

        }


        [Test]
        public void PostDeveRetornarErroAoGravarNoBanco()
        {
            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            var data = new List<Api.Usuario.Models.Usuario>
            {
                new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular ="952755705",
                Email = "neco@hotmail.com"
            },new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular ="952755705",
                Email = "neco@hotmail.com"
            },new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular ="952755705",
                Email = "neco@hotmail.com"
            }
            }.AsQueryable();

            //TODO: temos que voltar e entender melhor essa parte do MOCKSET

            var mockSet = new Mock<DbSet<Api.Usuario.Models.Usuario>>();
            mockSet.As<IQueryable<Api.Usuario.Models.Usuario>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Api.Usuario.Models.Usuario>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Api.Usuario.Models.Usuario>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Api.Usuario.Models.Usuario>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            var mockApplicationContext = new Mock<CP.Api.Usuario.ApplicationContext>();

            mockUsuarioRepository.Setup(p => p.Cadastrar(user)).Throws<DbUpdateException>();
            var usuarioRepository = mockUsuarioRepository.Object;
            var context = mockApplicationContext.Object;

            context.Usuarios = mockSet.Object;
            var controller = new UsuarioController( usuarioRepository);

            Assert.Throws<DbUpdateException>(() => controller.Post(user));

            //var retorno =  controller.Post(user);
            //var conflict = retorno.Result as ConflictResult;
            //Assert.IsNotNull(conflict);
            //Assert.AreEqual(409, conflict.StatusCode);

        }

        [Test]
        public void PostDeveRetornarConflito()
        {
            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };



            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(user)).Throws<Exception>();

            var usuarioRepository = mockUsuarioRepository.Object;

            var controller = new UsuarioController( usuarioRepository);


            Assert.Throws<Exception>(() => controller.Post(user));
        }

        [Test]
        public void PutDeveRetornarStatusCode204()
        {

            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            //var data = new List<User>
            //{
            //    new User
            //{
            //    Cpf = "12345678910",
            //    Nome = "Gustavo Rocha",
            //    Celular ="952755799",
            //    Email = "neco@hotmail.com"
            //},new User
            //{
            //    Cpf = "12345678910",
            //    Nome = "Gustavo Rocha",
            //    Celular ="952755705",
            //    Email = "neco@hotmail.com"
            //},new User
            //{
            //    Cpf = "12345678910",
            //    Nome = "Gustavo Rocha",
            //    Celular ="952755705",
            //    Email = "neco@hotmail.com"
            //}
            //}.AsQueryable();

            ////TODO: temos que voltar e entender melhor essa parte do MOCKSET

            //var mockSet = new Mock<DbSet<User>>();
            //mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            //mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            //mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            //mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            //var mockApplicationContext = new Mock<CP.Api.Usuario.ApplicationContext>();
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Alterar(usuario));
            mockUsuarioRepository.Setup(u => u.ConsultarPorParametro(usuario.Cpf)).Returns(usuario);

            var usuarioRepository = mockUsuarioRepository.Object;
            var context = new Api.Usuario.ApplicationContext(); 
           // context.Usuarios = mockSet.Object;

            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Put(usuario);

            var conflict = retorno.Result as NoContentResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(204, conflict.StatusCode);

        }

        [Test]
        public void PutDeveRetornarNotFoundAoAlterarUsuario()
        {

            var usuario = new Api.Usuario.Models.Usuario();


            


            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Alterar(usuario)).Verifiable();

            var usuarioRepository = mockUsuarioRepository.Object;

            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Put(usuario);

            var conflict = retorno.Result as NotFoundResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(404, conflict.StatusCode);

        }

        [Test]
        public void PutDeveRetornarErroAoGravarNoBanco()
        {

            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            


            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Alterar(usuario)).Throws<DbUpdateException>();
            mockUsuarioRepository.Setup(u => u.ConsultarPorParametro(usuario.Cpf)).Returns(usuario);

            var usuarioRepository = mockUsuarioRepository.Object;
            

            var controller = new UsuarioController( usuarioRepository);

            Assert.Throws<DbUpdateException>(() => controller.Put(usuario));

        }


        [Test]
        public void DeleteDeveExcluirUsuario()
        {

            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();

            mockUsuarioRepository.Setup(u => u.ConsultarPorParametro(usuario.Cpf)).Returns(usuario);
            mockUsuarioRepository.Setup(p => p.Excluir(usuario)).Verifiable();


            var usuarioRepository = mockUsuarioRepository.Object;
            
            


            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Delete(usuario.Cpf);

            var sucesso = retorno.Result as OkObjectResult;
            Assert.NotNull(sucesso);
            Assert.AreEqual(200, sucesso.StatusCode);

        }

        [Test]
        public void DeleteDeveRetornar404()
        {

            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();

            mockUsuarioRepository.Setup(u => u.ConsultarPorParametro(usuario.Cpf));
            mockUsuarioRepository.Setup(p => p.Excluir(usuario)).Verifiable();

            var usuarioRepository = mockUsuarioRepository.Object;
            
            

            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Delete(usuario.Cpf);

            var sucesso = retorno.Result as NotFoundResult;
            Assert.NotNull(sucesso);
            Assert.AreEqual(404, sucesso.StatusCode);


        }

        [Test]
        public void DeleteDeveREtornarUsuarioExcluido()
        {
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };


            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();

            mockUsuarioRepository.Setup(u => u.ConsultarPorParametro(usuario.Cpf)).Returns(usuario);
            mockUsuarioRepository.Setup(p => p.Excluir(usuario)).Verifiable();


            var usuarioRepository = mockUsuarioRepository.Object;
            


            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Delete(usuario.Cpf);


            var okResult = retorno.Result as OkObjectResult;
            var actualConfiguration = okResult.Value as Api.Usuario.Models.Usuario;

            Assert.AreEqual(usuario, actualConfiguration);

        }

        [Test]
        public void GetComParametroDeveRetornarOkAsync()
        {
            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.ConsultarPorParametro(user.Cpf)).Returns(user);
            var usuarioRepository = mockUsuarioRepository.Object;

            var controller = new UsuarioController( usuarioRepository);

            var retorno =  controller.Get(user.Cpf);

            var sucesso = retorno.Result as OkObjectResult;
            Assert.NotNull(sucesso);
            Assert.AreEqual(200, sucesso.StatusCode);


        }

        [Test]
        public void GetComParametroDeveRetornar404()
        {
            var cpf = "12345678910";
            var user = new Api.Usuario.Models.Usuario();

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.ConsultarPorParametro(null)).Returns(user);
            var usuarioRepository = mockUsuarioRepository.Object;

            var controller = new UsuarioController( usuarioRepository);

            var retorno = controller.Get(cpf);

            var sucesso = retorno.Result as NotFoundResult;
            Assert.NotNull(sucesso);
            Assert.AreEqual(404, sucesso.StatusCode);

        }

        [Test]
        public void GetComParametroDeveRetornarErroAoGravarNoBanco()
        {

            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.ConsultarPorParametro(user.Cpf)).Throws<DbUpdateException>();
            var usuarioRepository = mockUsuarioRepository.Object;

            var controller = new UsuarioController( usuarioRepository);

            Assert.Throws<DbUpdateException>(() => controller.Get(user.Cpf));

        }

    }
}