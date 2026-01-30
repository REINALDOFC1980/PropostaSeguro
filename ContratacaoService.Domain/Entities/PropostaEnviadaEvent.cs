namespace ContratacaoService.Domain.Entities;

public class PropostaEnviadaEvent
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = null!;
    public string TipoSeguro { get; set; } = null!;
    public decimal Valor { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CriadoEm { get; set; }
}
