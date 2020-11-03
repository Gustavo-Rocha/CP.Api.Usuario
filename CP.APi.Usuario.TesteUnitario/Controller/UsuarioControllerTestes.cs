using CP.Api.Usuario.Controllers;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CP.APi.Usuario.TesteUnitario
{
    public class UsuarioControllerTestes
    {
        [Test]
        public async Task DeveObterTodosUsuarios()
        {
            


            var usuarios = new List<Usuarios>()
{
    new Usuarios()
{
    Cpf = "123456+75"
},
    new Usuarios()
{
    Cpf = "4588565"
},
    new Usuarios()
{
    Cpf = "7854654"
},
    new Usuarios()
{
    Cpf = "5454546"
},
    new Usuarios()
{
    Cpf = "989898"
}
};
            //var controller  = new UsuarioController( new Api.Usuario.ApplicationContext()
            //    ,Mock.Of<IUsuarioRepository>(u => u.Consultar()== new List<Usuarios>()));


            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Returns( usuarios );
            var controller2 = new UsuarioController(new Api.Usuario.ApplicationContext(), mockUsuarioRepository.Object);

            var resposta = await controller2.Get();
         


            Assert.AreEqual(usuarios, resposta);

        }

        [Test]
        public async Task DeveRetornarListaVazia()
        {

            var listaVazia = new List<Usuarios>();

            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Returns(listaVazia);
            var controller2 = new UsuarioController(new Api.Usuario.ApplicationContext(), mockUsuarioRepository.Object);

            var resposta = await controller2.Get();


            Assert.IsEmpty(resposta);
        }

        [Test]
        public async Task DeveRetornarUmaExcecao()
        {
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(c => c.Consultar()).Throws<NullReferenceException>();
            var controller2 = new UsuarioController(new Api.Usuario.ApplicationContext(), mockUsuarioRepository.Object);

            //var resposta = await controller2.Get();

            	  
            Assert.ThrowsAsync<NullReferenceException>(()=>  controller2.Get());
           

        }

        [Test]
        public async Task PostDeveEfetuarCadastroDoUsuario()
        {
            var retorno = new Usuarios();
            
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(retorno)).Verifiable();

            var usuarioRepository = mockUsuarioRepository.Object;
            var context = new Api.Usuario.ApplicationContext();
            var controller = new UsuarioController(context, usuarioRepository);

              var returno = await controller.Post(retorno);

            
            Mock.Get(usuarioRepository).Verify(v => v.Cadastrar(It.IsAny<Usuarios>()),Times.Once);
            Mock.Get(usuarioRepository).Verify(u => u.Consultar(), Times.Exactly(2));
            Mock.Get(context).Verify(c => c.Usuarios.Any(c => c.Email == "Teste@teste"));
            //passa a instrução  verificar se  chamando esse metodo cadastra com  a variavel
            Assert.Pass("Funcionou");



        }
        [Test]
        public void PostDeveRetornarExcecao()
        {
            Usuarios usuarios = null;
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(null)).Throws<ArgumentException>();

            //o setup é como se fosse o cenário...
            var controller = new UsuarioController(new Api.Usuario.ApplicationContext(), mockUsuarioRepository.Object);

            Assert.ThrowsAsync<ArgumentException>(() => controller.Post(usuarios));

        }


        [Test]
        public void PostDeveRetornarErroAoGravarNoBanco()
        {
            var user = new Usuarios
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular ="952755705",
                Email = "neco@hotmail.com"
            };
            var mockUsuarioRepository = new Mock<IUsuarioRepository>() ;
            var mockApplicationContext = new Mock<CP.Api.Usuario.ApplicationContext>();

            mockUsuarioRepository.Setup(p => p.Cadastrar(user)).Verifiable();
            mockApplicationContext.Setup(c => c.Usuarios.Any(a=>a.Cpf== "12345678910") ==true).Verifiable();


            var usuarioRepository = mockUsuarioRepository.Object;
            var context = mockApplicationContext.Object;
            var controller = new UsuarioController(context, usuarioRepository);
            var returno =  controller.Post(user);

           // Mock.Get(usuarioRepository).Verify(v => v.Cadastrar(It.IsAny<Usuarios>()), Times.Once);
           // Mock.Get(context).SetupSet(s => s.Usuarios.Any(u => u.Cpf =="123456789")).Equals(true);   


            //Mock.Get(context).Verify(c => c.Usuarios.Any(c => c.Cpf == "12345678910"));
            
            //controller.Post(usuarios);

            Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => controller.Post(user));


        }

        [Test]
        public void PostDeveRetornarConflito()
        {
            var user = new Usuarios
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com"
            };
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            mockUsuarioRepository.Setup(p => p.Cadastrar(user)).Throws<Exception>();

            var usuarioRepository = mockUsuarioRepository.Object;
            //var context = mockApplicationContext.Object;
            var controller = new UsuarioController(new Api.Usuario.ApplicationContext(), usuarioRepository);
            var returno = controller.Post(user);

            Assert.ThrowsAsync<Exception>(() => controller.Post(user));

            //Mock.Get(usuarioRepository).Verify(v=>v.Cadastrar(user),Times.Once());
            
            //await controller.Post(retorno);

            //Assert.Pass("Funcionou");


        }

        





    }
}