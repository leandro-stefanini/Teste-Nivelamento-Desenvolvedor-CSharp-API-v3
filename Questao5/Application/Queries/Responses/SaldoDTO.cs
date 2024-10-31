namespace Questao5.Application.Queries.Responses
{
    public class SaldoDTO
    {
        public int NumeroConta { get; set; }
        public String NomeTitular { get; set; }
        public DateTime DataConsulta { get; set; } 
        public decimal SaldoAtual {  get; set; }    
    }
}
