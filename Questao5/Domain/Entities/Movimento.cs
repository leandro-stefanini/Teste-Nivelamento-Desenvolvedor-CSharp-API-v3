using System.Drawing;
using System;

namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        public string Idmovimento { get; set; }
        public string Idcontacorrente { get; set; }
        public string Datamovimento { get; set; }   
        public string Tipomovimento { get; set; }
        public string Valor { get; set; }
    }
}
