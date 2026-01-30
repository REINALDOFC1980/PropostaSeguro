// Validators/PropostaValidator.cs
using FluentValidation;
using PropostaService.Api.DTO;


namespace PropostaService.Api.Validators
{
    public class PropostaValidator : AbstractValidator<CriarPropostaRequest>
    {
        public PropostaValidator()
        {
            RuleFor(x => x.NomeCliente)
                .NotEmpty().WithMessage("Nome do cliente é obrigatório")
                .MaximumLength(150);

            RuleFor(x => x.TipoSeguro)
                .NotEmpty().WithMessage("Tipo de seguro é obrigatório")
                .MaximumLength(100);

            RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero")
            .Must(v => decimal.Round(v, 2) == v)
            .WithMessage("Valor deve ter no máximo duas casas decimais");

        }
    }
}
