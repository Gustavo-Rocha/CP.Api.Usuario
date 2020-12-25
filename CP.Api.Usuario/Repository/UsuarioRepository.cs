using CP.Api.Usuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CP.Api.Usuario.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationContext _context;

        public UsuarioRepository(ApplicationContext context)
        {
            _context = context;
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

            this.DetachLocal(_ => _.Cpf == usuario.Cpf);
            



            //_context.Entry<Models.Usuario>(usuario).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            _context.Usuarios.Update(usuario);
            //_context.Entry(usuario).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            //_context.Entry<Models.Usuario>(usuario).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            _context.SaveChanges();
        }

        public void Cadastrar(Models.Usuario usuario)
        {


            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }
            catch (Exception e)
            {

                throw e.InnerException;
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

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

        }
    }
}
