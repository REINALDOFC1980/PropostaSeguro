namespace PropostaService.Domain.Entities
{
    public enum StatusProposta
    {
        EmAnalise,
        Aprovada,
        Rejeitada
    }

    public class PropostaModel
    {
        public Guid Id { get; set; }
        public string NomeCliente { get; set; } = null!;
        public string TipoSeguro { get; set; } = null!;
        public decimal Valor { get; set; }      
        public StatusProposta Status { get; set; } = StatusProposta.EmAnalise;
        public DateTime CriadoEm { get; set; }
    }
}
