using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CP.Api.Usuario.Models;
using CP.Api.Usuario.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace CP.Api.Usuario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioController(ApplicationContext context,IUsuarioRepository usuarioRepository)
        {
            _context = context;
            this.usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Tras todos os usuários cadastrados
        /// em Uma lista para serem exibidos.
        /// </summary>
        /// <param name="temperatura">Temperatura em Fahrenheit</param>
        /// <returns>Retorna os usuários cadastrados no banco de dados</returns>

        // GET: api/Usuario
        [HttpGet]
        public async Task<IEnumerable<Usuarios>> Get()
        {
            var retorno =  usuarioRepository.Consultar();
            return retorno;

        }
        
        
         /// <summary>
         /// Tras o usuário cadastrado
         /// </summary>
         /// <param name="Cpf">CPF usado para identificação do usuário</param>
         /// <returns>Retorna os usuários cadastrados no banco de dados</returns>
        // GET: api/Usuario/5
        [HttpGet("{Cpf}")]
        public async Task<ActionResult<Usuarios>> Get([Required]string Cpf)
        {
                 
            var usuarios = usuarioRepository.ConsultarPorParametro(Cpf);
           
            if (usuarios == null)
            {
                return NotFound();
            }

            return usuarios;
        }

        /// <summary>
        /// Tras o usuário cadastrado
        /// </summary>
        /// <param name="Cpf">CPF usado para identificação do usuário</param>
        /// <param name="usuarios">Objeto utilizado para ser inserido na base, com a atualização </param>
        /// <returns>Retorna o usuário cadastrados </returns>

        // PUT: api/Usuario/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Usuarios usuarios)
        {

            
                //if (ModelState.IsValid)
               // {
                    if (!UsuariosExists(usuarios.Cpf))
                    {
                        return NotFound();
                    }

                    usuarioRepository.Alterar(usuarios);
                //}               
          
            

            return NoContent();
        }

        
        /// <summary>
        /// Tras o usuário cadastrado
        /// </summary>
        /// <param name="Usuario">Objeto usado para cadastrar o usuario</param>
        /// <returns>Retorna o usuário cadastrados </returns>

        // POST: api/Usuario
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Usuarios>> Post(Usuarios usuarios)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    usuarioRepository.Cadastrar(usuarios);
                }
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
        /// Tras o usuário cadastrado
        /// </summary>
        /// <param name="Cpf">CPF usado para exclusão do usuário</param>
        /// <returns>Retorna os usuários cadastrados no banco de dados</returns>

        // DELETE: api/Usuario/5
        [HttpDelete("{Cpf}")]
        public async Task<ActionResult<Usuarios>> Delete(string Cpf)
        {
            var usuarios = usuarioRepository.ConsultarPorParametro(Cpf);
            if (usuarios == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                usuarioRepository.Excluir(usuarios);
            }

            return usuarios;
        }

        private bool UsuariosExists(string Cpf)
        {
            return _context.Usuarios.Any(e => e.Cpf == Cpf);
        }
    }

   
}
