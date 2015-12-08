using System;

namespace Naive_Bayes.Models
{
    public class ResultadoSencillo
    {
        public int TotalPalabrasPositivas { get; set; }
        public int TotalPalabrasNegativas { get; set; }
        public double ProbabilidadPositivaP { get; set; }
        public double ProbabilidadNegativaN { get; set; }
        public TimeSpan Duracion { get; set; }
    }
}
