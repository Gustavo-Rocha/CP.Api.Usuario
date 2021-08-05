using CP.Api.Usuario.Controllers;
using CP.Api.Usuario.Criptografia;
using CP.Api.Usuario.EmailService;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using CP.Api.Usuario.TokenJWT;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CP.APi.Usuario.TesteUnitario.Controller
{
    public class LoginControllerTestes
    {
        private readonly Mock<IUsuarioRepository> mockUsuarioRepository;
        private readonly Mock<ICriptografar> mockCriptografar;
        private readonly LoginController loginController;
        private readonly Mock<IUsuarioRepository> usuarioRepository;
        private readonly Mock<ITokenService> mockTokenService;
        private readonly Mock<IEmailSender> mockEmailSender;
        private readonly Mock<SmtpClient> mockSmtpClient;
        private readonly Mock<MailMessage> mockMailmessage;

        public LoginControllerTestes()
        {
            
        }

        [Test]
        public async Task  Login_Deve_Retornar_True()
        {
            //Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IklyaW5ldSIsInJvbGUiOiIzODgzMDA2NzgwOSIsIm5iZiI6MTYxODYxMTcwMCwiZXhwIjoxNjE4NjE4OTAwLCJpYXQiOjE2MTg2MTE3MDB9.BgJqunH22zAOW0JIrjyKHQCqvy_IGA8BpyyIDC5yyRU";
            var retornoLogin = new RetornoLogin 
            { 
                Token = token,
                Valido = true 
            };
            var usuario = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com",
                Senha = "240497gu"
            };

            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu"
            };


            var mockCriptografar = new Mock<ICriptografar>();
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockEmailSender = new Mock<IEmailSender>();

            mockUsuarioRepository.Setup(p => p.ConsultarPorParametro(usuario.Cpf)).Returns(usuario);
            mockTokenService.Setup(_ => _.GenerateToken(usuario)).Returns(token);
            mockCriptografar.Setup(_ => _.Descriptografar(usuario.Cpf, usuario.Senha)).Returns(true);
            mockEmailSender.Setup(_ => _.SendEmail("gustavooliveirarocha@hotmail.com", usuario.Email)).Returns(true);
            // mockCriptografar.Setup(_ => _.Descriptografar(user.Cpf, user.Senha)).Returns(false);
            var loginController = new LoginController(mockCriptografar.Object, mockUsuarioRepository.Object, mockEmailSender.Object, mockTokenService.Object);
            
            //Act

            var responseLoginController = await loginController.Login(login);
            var conflict = responseLoginController.Result as OkObjectResult;

            //Assert

            conflict.StatusCode.Should().Be(200);
            //conflict.Value.Should().Be(retornoLogin.Token);
            //conflict.Value.Should().Be(true);

        }

        [Test]
        public async Task Login_Deve_Retornar_False()
        {
            //Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IklyaW5ldSIsInJvbGUiOiIzODgzMDA2NzgwOSIsIm5iZiI6MTYxODYxMTcwMCwiZXhwIjoxNjE4NjE4OTAwLCJpYXQiOjE2MTg2MTE3MDB9.BgJqunH22zAOW0JIrjyKHQCqvy_IGA8BpyyIDC5yyRU";
            var retornoLogin = new RetornoLogin
            {
                Token = token,
                Valido = true
            };

            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com",
                Senha = "240497gu"
            };

            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu"
            };

            var mockCriptografar = new Mock<ICriptografar>();
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockEmailSender = new Mock<IEmailSender>();

            mockUsuarioRepository.Setup(p => p.ConsultarPorParametro(user.Cpf)).Returns(user);
            mockTokenService.Setup(_ => _.GenerateToken(user)).Returns(token);
            mockCriptografar.Setup(_ => _.Descriptografar(user.Cpf, user.Senha)).Returns(false);
            mockEmailSender.Setup(_ => _.SendEmail("gustavooliveirarocha@hotmail.com", user.Email)).Returns(true);
            // mockCriptografar.Setup(_ => _.Descriptografar(user.Cpf, user.Senha)).Returns(false);
            var loginController = new LoginController(mockCriptografar.Object, mockUsuarioRepository.Object, mockEmailSender.Object, mockTokenService.Object);
            //Act

            var responseLoginController = await loginController.Login(login);

            var conflict = responseLoginController.Result as NotFoundObjectResult;

            //Assert

            conflict.StatusCode.Value.Should().Be(404);
            //responseLoginController.Value.Token.Should().Contain(token);
            //conflict.StatusCode.Should().
            

        }
        [Test]
        public async Task RecuperarSenha_Deve_Retornar_OK()
        {
          var  mockCriptografar = new Mock<ICriptografar>();
          var mockUsuarioRepository = new Mock<IUsuarioRepository>();
          var mockTokenService = new Mock<ITokenService>();
          var mockEmailSender = new Mock<IEmailSender>();

            var user = new Api.Usuario.Models.Usuario
            {
                Cpf = "12345678910",
                Nome = "Gustavo Rocha",
                Celular = "952755705",
                Email = "neco@hotmail.com",
                Senha = "240497gu"
            };

            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu",
                Email = "gustavooliveira@hotmail.com"
            };
            var email = "gustavooliveira@hotmail.com";

            mockUsuarioRepository.Setup(_ => _.ConsultarPorParametro(login.Cpf)).Returns(user);
            mockEmailSender.Setup(_ => _.SendEmail(It.IsAny<string>(), email)).Returns(true);
          var loginController = new LoginController(mockCriptografar.Object, mockUsuarioRepository.Object, mockEmailSender.Object, mockTokenService.Object);

          var responseLoginController =  await loginController.RecuperarSenha(login);

            var statusCode = responseLoginController as OkResult;
            statusCode.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task RecuperarSenha_Deve_Retornar_BadRequest()
        {
            var mockCriptografar = new Mock<ICriptografar>();
            var mockUsuarioRepository = new Mock<IUsuarioRepository>();
            var mockTokenService = new Mock<ITokenService>();
            var mockEmailSender = new Mock<IEmailSender>();
            var login = new Api.Usuario.Models.LoginModel
            {
                //Cpf = new Random().Next(0,999999999).ToString("00000000000"),
                Cpf = "12345678910",
                Senha = "240497gu",
                Email = "gustavooliveira@hotmail.com"
            };
            var email = "gustavooliveira@hotmail.com";

            mockEmailSender.Setup(_ => _.SendEmail(It.IsAny<string>(), email)).Returns(false);
            var loginController = new LoginController(mockCriptografar.Object, mockUsuarioRepository.Object, mockEmailSender.Object, mockTokenService.Object);

            var responseLoginController = await loginController.RecuperarSenha(login);

            var statusCode = responseLoginController as BadRequestResult;
            statusCode.StatusCode.Should().Be(400);
        }

    }
}
//[Fact]
//public void GetOrders_WithOrdersInRepo_ReturnsOk()
//{
//    // arrange
//    var controller = new OrdersController(new MockRepository());

//    // act
//    var result = controller.GetOrders();
//    var okResult = result as OkObjectResult;

//    // assert
//    Assert.IsNotNull(okResult);
//    Assert.AreEqual(200, okResult.StatusCode);
//}
//StatusCodes.Status200OK
