using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContratacaoService.Domain.Entities
{
    public class PropostaMessage
    {
        public Guid Id { get; set; }
        public string NomeCliente { get; set; } = null!;
        public string TipoSeguro { get; set; } = null!;
        public decimal Valor { get; set; }
        public DateTime CriadoEm { get; set; }
    }

}
