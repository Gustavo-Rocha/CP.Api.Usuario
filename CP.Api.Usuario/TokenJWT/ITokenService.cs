using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CP.Api.Usuario.TokenJWT
{
    public interface ITokenService
    {
         string GenerateToken(Models.Usuario user);
    }
}
