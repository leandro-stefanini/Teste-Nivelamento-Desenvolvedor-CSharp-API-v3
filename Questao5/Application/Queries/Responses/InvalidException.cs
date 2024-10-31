namespace Questao5.Application.Queries.Responses
{
    public class InvalidException: Exception
    {
        public InvalidException(string InvalidContext): base(InvalidContext) { }
    }
}
