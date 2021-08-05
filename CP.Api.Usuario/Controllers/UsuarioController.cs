using AutoMapper;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuarioRepository usuarioRepository;
        private HashAlgorithm algoritmo;
        private readonly IMapper mapper;
        public UsuarioController(IUsuarioRepository usuarioRepository,IMapper mapper)
        {
            this.usuarioRepository = usuarioRepository;
            this.mapper = mapper; 
        }

        /// <summary>
        /// Tras todos os usuários cadastrados
        /// em Uma lista para serem exibidos.
        /// </summary>
        /// <returns>Retorna os usuários cadastrados no banco de dados</returns>

        // GET: api/Usuario
        [HttpGet]
        //[Authorize]
        public ActionResult<IEnumerable<Models.Usuario>> Get()
        {

            var retorno =  usuarioRepository.Consultar();
            return Ok(retorno);

        }

        /// <summary>
        /// Tras o usuário cadastrado
        /// </summary>
        /// <param name="Cpf">CPF usado para identificação do usuário</param>
        /// <returns>Retorna os usuários cadastrados no banco de dados</returns>
        // GET: api/Usuario/5
        [HttpGet("{Cpf}")]
        [Authorize]
        public ActionResult<Models.Usuario> Get([Required] string Cpf)
        {
            var usuarios = usuarioRepository.ConsultarPorParametro(Cpf);

            if (usuarios == null)
            {
                return NotFound();
            }

            return Ok(usuarios);
        }

        /// <summary>
        /// Altera o usuário no Banco de dados
        /// </summary>
        /// <param name="usuarios">Objeto utilizado para ser inserido na base, com a atualização </param>
        /// <returns>StatusCode 204 </returns>

        // PUT: api/Usuario/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        [Authorize]
        public ActionResult<Models.Usuario> Put([FromBody] Models.UsuarioViewModel usuariosViewModel)
        {
            
            if (!UsuariosExists(usuariosViewModel.Cpf))
            {
                return NotFound();
            }

            var usuarios = mapper.Map<Models.Usuario>(usuariosViewModel);

            usuarioRepository.Alterar(usuarios);

            return NoContent();
        }

        /// <summary>
        /// Tras o usuário cadastrado
        /// </summary>
        /// <param name="usuarios">Objeto usado para cadastrar o usuario</param>
        /// <returns>Retorna o usuário cadastrados </returns>

        // POST: api/Usuario
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        
        public ActionResult<Models.Usuario> Post(Models.UsuarioViewModel usuariosViewModel)
        {
            var usuarios = mapper.Map<Models.Usuario>(usuariosViewModel);
            try
            {
                
                usuarioRepository.Cadastrar(usuarios);  
            }
            catch (DbUpdateException)
            {
                if (UsuariosExists(usuarios.Cpf))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { Cpf = usuarios.Cpf }, usuarios);
        }

        /// <summary>
        /// Deleta o usuário da base de dados
        /// </summary>
        /// <param name="Cpf">CPF usado para encontrar usuario na base e excluí-lo</param>
        /// <returns>Retorna o usuário excluído do banco de dados</returns>

        // DELETE: api/Usuario/5
        [HttpDelete("{Cpf}")]
        [Authorize]
        public ActionResult<Models.Usuario> Delete(string Cpf)
         {
            var usuarios = usuarioRepository.ConsultarPorParametro(Cpf);
            if (usuarios == null)
            {
                return NotFound();
            }

            usuarioRepository.Excluir(usuarios);

            return Ok(usuarios);
        }
        private bool UsuariosExists(string Cpf)
        {

            var usuario = usuarioRepository.ConsultarPorParametro(Cpf);
            
            return usuario != null;

        }
    }
}
