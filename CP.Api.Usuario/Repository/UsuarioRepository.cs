using CP.Api.Usuario.Criptografia;
using CP.Api.Usuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CP.Api.Usuario.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationContext _context;
        private readonly IHash256 hash;

        public UsuarioRepository(ApplicationContext context,IHash256 hash)
        {
            _context = context;
            this.hash = hash;
        }

        public virtual void DetachLocal(Func<Models.Usuario, bool> predicate)
        {
            var local = _context.Set<Models.Usuario>().Local.Where(predicate).FirstOrDefault();
            if(!local.Equals(null))
            {
                _context.Entry(local).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

        public void Alterar(Models.Usuario usuario)
        {
            var UsuarioAntigo = ConsultarPorParametro(usuario.Cpf);
            var senhaAtual = usuario.Senha;

            usuario.Senha = hash.CriptografarSenha(senhaAtual);

            this.DetachLocal(_ => _.Cpf == usuario.Cpf);

            _context.Usuarios.Update(usuario);
            _context.SaveChanges();
        }

        public void Cadastrar(Models.Usuario usuario)
        {
            try
            {
                usuario.Senha = hash.CriptografarSenha(usuario.Senha);
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public List<Models.Usuario> Consultar()
        {
            return _context.Usuarios.ToList();
        }

        public Models.Usuario ConsultarPorParametro(string cpf)
        {
            return _context.Usuarios.Find(cpf);
        }

        public void Excluir(Models.Usuario usuario)
        {
            this.DetachLocal(_ => _.Cpf == usuario.Cpf);

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
        }
    }
}
