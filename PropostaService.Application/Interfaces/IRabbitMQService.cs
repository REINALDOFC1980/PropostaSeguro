using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropostaService.Application.Interfaces
{
    // Application/Interfaces/IRabbitMQService.cs
    public interface IRabbitMQService
    {
        void EnviarProposta(PropostaEnviadaEvent proposta);
    }

}
