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

        // Valor default em memória (opcional, mas não precisa)
        public StatusProposta Status { get; set; } = StatusProposta.EmAnalise;

        // CriadoEm será definido pelo banco
        public DateTime CriadoEm { get; set; }
    }
}
