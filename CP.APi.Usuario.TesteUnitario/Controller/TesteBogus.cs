using AutoBogus;
using System;
using System.Collections.Generic;
using System.Text;
using Bogus.Extensions.Brazil;

namespace CP.APi.Usuario.TesteUnitario.Controller
{
    class TesteBogus :AutoFaker<Api.Usuario.Models.UsuarioViewModel> 
    {

        public TesteBogus()
        {
            RuleFor(_ => _.Cpf, _ => _.Person.Cpf(false));
            RuleFor(_ => _.Celular, _ => _.Person.Phone);
            RuleFor(_ => _.Email, _ => _.Person.Email);
            RuleFor(_ => _.Nome, _ => _.Person.FirstName);
            RuleFor(_ => _.Senha, _ => _.Internet.Password(8));
        }



        //public static readonly Faker<ComplementoAuditoria> ComplementoAuditoria = new Faker<ComplementoAuditoria>()
        //    .StrictMode(true)
        //    .RuleFor(r => r.Extensao1, f => f.Lorem.Text())
        //    .RuleFor(r => r.Extensao2, f => f.Lorem.Text())
        //    .RuleFor(r => r.Extensao3, f => f.Lorem.Text())
        //    .RuleFor(r => r.Extensao4, f => f.Lorem.Text())
        //    .RuleFor(r => r.Extensao5, f => f.Lorem.Text());

        //public static readonly Faker<RegistroAuditoriaLegado




        //    RuleFor(p => p.ChKey, v => v.Lorem.Text());
        //    RuleFor(p => p.TotalTime, v => v.Random.Short());
        //RuleFor(p => p.NumberOfChallenges, v => v.Random.Short());
        //RuleFor(p => p.SnapFrequenceInMillis, v => v.Random.Int());
        //RuleFor(p => p.SnapNumber, v => v.Random.Short());
    }
}
