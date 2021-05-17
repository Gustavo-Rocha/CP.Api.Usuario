using CP.Api.Usuario.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Criptografia
{
    public class Criptografar:ICriptografar
    {
        
        private readonly IHash256 hash;
        private readonly IUsuarioRepository usuarioRepository;
        public Criptografar( IHash256 hash256, IUsuarioRepository usuarioRepository)
        {
            this.hash = hash256;
            this.usuarioRepository = usuarioRepository;
        }

        public void Criptgrafar(string senha)
        {
            hash.CriptografarSenha(senha);
        }

        public bool Descriptografar(string cpf,string senhaDigitada)
        {
            var usuario = usuarioRepository.ConsultarPorParametro(cpf);

            if(usuario == null)
            {
                return false;
            }

            hash.CriptografarSenha(usuario.Senha);
            var senhaCadastro = usuario.Senha;
            senhaDigitada = hash.CriptografarSenha(senhaDigitada);

            if ((senhaDigitada == senhaCadastro))
            {
                return true;
            }

            return false;
        }
    }
}
