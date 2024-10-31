using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using System.Data.Common;

namespace Questao5.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        void Setup();
        SaldoDTO Saldo(int numeroConta);
        string OperacaoMovimentacao(int numeroConta, decimal valor, char tipo);
        IdEmpotencia? IdEmpotencia(string chave_idempotencia);
        void CriarIdEmpotencia(string chave_idempotencia, string requisicao, string resultado);
    }
}