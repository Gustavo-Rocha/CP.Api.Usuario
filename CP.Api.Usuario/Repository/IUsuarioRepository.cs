
using System.Collections.Generic;

namespace CP.Api.Usuario.Repository
{
    public interface IUsuarioRepository
    {

        void Cadastrar(Models.Usuario Usuario);
        void Excluir(Models.Usuario usuario);
        void Alterar(Models.Usuario usuario);
        List<Models.Usuario> Consultar();
        Models.Usuario ConsultarPorParametro(string cpf);

    }
}
