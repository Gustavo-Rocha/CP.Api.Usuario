using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Criptografia
{
    public interface IHash256
    {
        string CriptografarSenha(string senha);

        
    }
}
