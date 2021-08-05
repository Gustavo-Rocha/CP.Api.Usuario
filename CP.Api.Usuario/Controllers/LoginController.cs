using CP.Api.Usuario.Criptografia;
using CP.Api.Usuario.EmailConfiguration;
using CP.Api.Usuario.EmailService;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using CP.Api.Usuario.TokenJWT;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace CP.Api.Usuario.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]

    public class LoginController : ControllerBase
    {

        private readonly ICriptografar criptografarSenha;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IEmailSender emailSender;
        private readonly ITokenService tokenService;


        public LoginController(ICriptografar criptografarSenha, IUsuarioRepository usuarioRepository, IEmailSender emailSender, ITokenService tokenService)
        {

            this.criptografarSenha = criptografarSenha;
            this.usuarioRepository = usuarioRepository;
            this.emailSender = emailSender;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<RetornoLogin>> Login([Required] LoginModel loginModel)
        {
            var comparaSenha = criptografarSenha.Descriptografar(loginModel.Cpf, loginModel.Senha);

            var usuario = usuarioRepository.ConsultarPorParametro(loginModel.Cpf);

            // Verifica se o usuário existe
            if ((usuario == null) || (comparaSenha == false))
            {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }

            var token = tokenService.GenerateToken(usuario);

            // Retorna os dados
            return Ok( new RetornoLogin
            {
                Token = token,
                Valido =comparaSenha
            });
        }
        [Route("RecuperarSenha")]
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha([Required] LoginModel loginModel)
        {
            var NovaSenha = new Random().Next(0, 999999999).ToString("00000000");
            var resposta = emailSender.SendEmail(NovaSenha, loginModel.Email);
            
            try
            {
                if (resposta)
                {
                    var usuarios = usuarioRepository.ConsultarPorParametro(loginModel.Cpf);
                    usuarios.Senha = NovaSenha;
                    usuarioRepository.Alterar(usuarios);
                    return Ok();
                }   
            }
            catch (Exception e )
            {
                throw e ;
            }
            return BadRequest();
        }
    }
}
