namespace PropostaService.Api.DTO
{
    public class CriarPropostaRequest
    {
        public string NomeCliente { get; set; } = null!;
        public string TipoSeguro { get; set; } = null!;
        public decimal Valor { get; set; }
    }

}
