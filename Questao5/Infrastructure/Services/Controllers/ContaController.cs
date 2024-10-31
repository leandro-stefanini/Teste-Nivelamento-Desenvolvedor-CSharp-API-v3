using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Queries.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContaController : ControllerBase
    {
        IDatabaseBootstrap dtContext { get; }
        public ContaController(IDatabaseBootstrap cfg) {
            this.dtContext = cfg;
        }
        [HttpPut("Operacao")]
        public IActionResult Operacao([FromHeader(Name = "Idempotency-Key")] string chave_idempotencia, int numeroConta, decimal valor, char tipo)
        {
            /*
             A API Conta/Operacao é responsável pela manipulação dos valores da conta.
             Para proteção de ações repetitivas é necessário uma CHAVE_IPEMPOTENCIA que deve ser enviada pela requisição.
             Os Parâmetros para operação são: NumeroConta (Valor Númerico), Valor (Valor Númerico a ser incluído na movimentação) e o Tipo.
             TIPO: (C) - Crédito, quando for incrementado de valor no saldo
             TIPO: (D) - Débito, quando for decrementado o valor no saldo.

            Caso haja falha nas validações o sistema irá retornar:
            INVALID_ACCOUNT - Número da conta invalido;
            INACTIVE_ACCOUNT - Conta Inativa;
            INVALID_VALUE - Valor da Operação invalido;
            INVALID_TYPE - Tipo da Operação invalido;             
             */
            try
            {
                if (string.IsNullOrEmpty(chave_idempotencia))
                    return BadRequest("Chave de idempotência não informada");

                var idEmpotencia = this.dtContext.IdEmpotencia(chave_idempotencia);
                if (idEmpotencia != null)
                    return Ok(idEmpotencia.Resultado);

                var idMovimentacao = this.dtContext.OperacaoMovimentacao(numeroConta, valor, tipo);

                this.dtContext.CriarIdEmpotencia(chave_idempotencia, "", idMovimentacao);

                return Ok(idMovimentacao);
            }
            catch (InvalidException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("Saldo/{numeroConta}")]
        public IActionResult Saldo(int numeroConta)
        {
            /*
            A API Conta/Saldo, retorna o saldo de determinada conta bancária informada pelo parâmetro (NumeroConta - Tipo Número).
            Retorna o Saldo Bancário, resultado dos valores de crédito menos o valor de débito.
            Caso haja falha nas validações o sistema irá retornar:
            INVALID_ACCOUNT - Número da conta invalido;
            INACTIVE_ACCOUNT - Conta Inativa
             */
            try
            {
                var dadosSaldo = this.dtContext.Saldo(numeroConta);
                
                return Ok(dadosSaldo);

            }
            catch (InvalidException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Dados não encontrados!");
            }
        }
    }
}
