using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        private int Numero { get; }
        private string Titular { get; set; }
        private double DepositoInicial { get;  }
        private double Saldo { get; set; }
        private double TaxaSaque { get; set; }

        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            this.TaxaSaque = 3.50;
            this.Numero = numero;
            this.Titular = titular;
            this.DepositoInicial = depositoInicial;
            this.Saldo = depositoInicial;
        }
        public void Deposito(double valor)
        {            
            this.Saldo += valor;
        }
        public void Saque(double valor)
        {
            this.Saldo -= valor;
            this.Saldo -= this.TaxaSaque;
        }

        public static implicit operator string(ContaBancaria c) => c.ToString();
        public override string ToString()
            => $"Conta {this.Numero}, Titular: {this.Titular}, Saldo: $ {this.Saldo.ToString("0.00", CultureInfo.InvariantCulture)}";
    }
}
