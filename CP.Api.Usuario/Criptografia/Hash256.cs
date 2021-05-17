using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Criptografia
{
    public class Hash256: IHash256
    {

        private readonly HashAlgorithm algoritmo;
        public Hash256(HashAlgorithm algoritmo)
        {
            this.algoritmo = algoritmo;
        }

        public  string  CriptografarSenha(string senha )
        {
            var encodedValue = Encoding.UTF8.GetBytes(senha);
            var encryptedPassword = algoritmo.ComputeHash(encodedValue);

            var sb = new StringBuilder();
            foreach (var caracter in encryptedPassword)
            {
                sb.Append(caracter.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
