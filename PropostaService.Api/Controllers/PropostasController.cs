using Microsoft.AspNetCore.Mvc;
using PropostaService.Api.DTO;
using PropostaService.Application.Services;
using PropostaService.Domain.Entities;

namespace PropostaService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropostasController : ControllerBase
    {
        private readonly PropostaServiceApp _service;

        public PropostasController(PropostaServiceApp service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPropostaRequest request)
        {
            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            var proposta = await _service.CriarPropostaAsync(request.NomeCliente, request.TipoSeguro, request.Valor, idempotencyKey);
            return CreatedAtAction(nameof(ObterPorId), new { id = proposta.Id }, proposta);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var propostas = await _service.ListarPropostasAsync();
            return Ok(propostas);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var proposta = await _service.AlterarStatusAsync(id, StatusProposta.EmAnalise);
            if (proposta == null) return NotFound();
            return Ok(proposta);
        }

        /// <summary>
        /// Altera o status de uma proposta
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusRequest request)
        {
            var proposta = await _service.AlterarStatusAsync(id, request.Status);
            if (proposta == null) return NotFound(new { message = "Proposta não encontrada" });

            return Ok(proposta);
        }


    }

   
   
}
