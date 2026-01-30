using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropostaService.Domain.Entities
{
    public class IdempotencyKey
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid PropostaId { get; set; }
    }

}
