using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Criptografia
{
    public interface ICriptografar
    {

        void Criptgrafar(string senha);
        bool Descriptografar(string cpf, string senhaDigitada);

    }
}
