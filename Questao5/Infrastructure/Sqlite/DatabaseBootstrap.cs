using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using System;
using System.Data.Common;
using System.Drawing;
using System.Globalization;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'contacorrente' or name = 'movimento' or name = 'idempotencia');");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && (tableName == "contacorrente" || tableName == "movimento" || tableName == "idempotencia"))
                return;

            connection.Execute("CREATE TABLE contacorrente ( " +
                               "idcontacorrente TEXT(37) PRIMARY KEY," +
                               "numero INTEGER(10) NOT NULL UNIQUE," +
                               "nome TEXT(100) NOT NULL," +
                               "ativo INTEGER(1) NOT NULL default 0," +
                               "CHECK(ativo in (0, 1)) " +
                               ");");

            connection.Execute("CREATE TABLE movimento ( " +
                "idmovimento TEXT(37) PRIMARY KEY," +
                "idcontacorrente TEXT(37) NOT NULL," +
                "datamovimento TEXT(25) NOT NULL," +
                "tipomovimento TEXT(1) NOT NULL," +
                "valor REAL NOT NULL," +
                "CHECK(tipomovimento in ('C', 'D')), " +
                "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente) " +
                ");");

            connection.Execute("CREATE TABLE idempotencia (" +
                               "chave_idempotencia TEXT(37) PRIMARY KEY," +
                               "requisicao TEXT(1000)," +
                               "resultado TEXT(1000));");

            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('B6BAFC09-6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);");

        }

        public SaldoDTO Saldo(int numeroConta) {
            using var connection = new SqliteConnection(databaseConfig.Name);

            Func<string, decimal> convertNumber = (valor) =>
            {
                return Convert.ToDecimal(valor, new CultureInfo("pt-BR"));
            };
            
            var conta = connection.Query<ContaCorrente>($"SELECT * FROM contacorrente WHERE numero = {numeroConta} ").FirstOrDefault();

            if (conta == null)
            {
                throw new InvalidException("INVALID_ACCOUNT"); 
            }
            else if (conta.Ativo == 0)
            {
                throw new InvalidException("INACTIVE_ACCOUNT"); 
            }

            var movimentos = connection.Query<Movimento>($"SELECT * FROM movimento as M INNER JOIN contacorrente as C ON M.idcontacorrente = C.idcontacorrente WHERE numero = {numeroConta} ");

            var credito = movimentos.Where(m => m.Tipomovimento == "C").Select(s => convertNumber(s.Valor)).Sum(s => s);
            var debito = movimentos.Where(m => m.Tipomovimento == "D").Select(s => convertNumber(s.Valor)).Sum(s => s);
            var saldoAtual = credito - debito;

            return new SaldoDTO()
            {
                NomeTitular = conta.Nome,
                DataConsulta = DateTime.Now,
                NumeroConta = conta.Numero,
                SaldoAtual = saldoAtual
            };
        }

        public string OperacaoMovimentacao(int numeroConta, decimal valor, char tipo)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var conta = connection.Query<ContaCorrente>($"SELECT * FROM contacorrente WHERE numero = {numeroConta} ").FirstOrDefault();

            if(conta == null)
            {
                throw new InvalidException("INVALID_ACCOUNT");
            }
            else if (conta.Ativo == 0)
            {
                throw new InvalidException("INACTIVE_ACCOUNT"); 
            }
            else if (valor < 0)
            {
                throw new InvalidException("INVALID_VALUE");  ; 
            }
            else if(tipo != 'C' && tipo != 'D')
            {
                throw new InvalidException("INVALID_TYPE"); 
            }
            var guid = Guid.NewGuid().ToString().ToUpper();

            connection.Execute(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) " +
                $"VALUES('{guid}', '{conta.IdcontaCorrente}', '{DateTime.Now.ToString("dd/MM/yyyy")}', '{tipo}', '{valor}');");

            return guid;
        }

        public IdEmpotencia? IdEmpotencia(string chave_idempotencia)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            var ide_ = connection.Query<IdEmpotencia>($"SELECT * FROM IdEmpotencia WHERE chave_idempotencia = {chave_idempotencia} ").FirstOrDefault();
            return ide_;
        }

        public void CriarIdEmpotencia(string chave_idempotencia, string requisicao, string resultado)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            connection.Execute(
                "INSERT INTO IdEmpotencia (chave_idempotencia, chave_idempotencia, resultado ) " +
                $"VALUES('{chave_idempotencia}', '{requisicao}', '{resultado}')");
        }
    }
}
