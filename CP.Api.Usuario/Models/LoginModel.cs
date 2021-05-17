using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CP.Api.Usuario.Models
{
    public class LoginModel
    {
        [StringLength(11, MinimumLength = 11)]
        public string Cpf { get; set; }

        [StringLength(8, MinimumLength = 6)]
        public string Senha { get; set; }

        [StringLength(60, MinimumLength = 6)]
        public string Email { get; set; }


    }
}
