using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CP.Api.Usuario
{
    public class HealthCheck : IHealthCheck
    {
        private HttpClient _client;
        public HealthCheck()
        {
              
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
             
            HttpResponseMessage response = await _client.GetAsync("/api/Usuario");
            string usuarios = await response.Content.ReadAsStringAsync();
            var conversao = JsonConvert.DeserializeObject<List<Api.Usuario.Models.Usuario>>(usuarios);

            var sucesso = response.StatusCode.ToString();


            if (sucesso != "200")
            {
                return await Task.FromResult(
                HealthCheckResult.Unhealthy("An unhealthy result."));
            }
            else
            {
                return await Task.FromResult(
                HealthCheckResult.Healthy("A healthy result."));
            }
            


        }
    }
}
