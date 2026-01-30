namespace ContratacaoService.Domain.Entities
{
    public class ContratacaoModel
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public string NomeCliente { get; set; }
        public string TipoSeguro { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime CriadoEm { get; set; }
    }

}
