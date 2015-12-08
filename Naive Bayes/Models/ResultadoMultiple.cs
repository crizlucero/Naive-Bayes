namespace Naive_Bayes.Models
{
    public class ResultadoMultiple : ResultadoSencillo
    {
        public int TotalTuits { get; set; }
        public double TotalPositivaP { get; set; }
        public double TotalNegativaP { get; set; }
        public double TotalPositivaN { get; set; }
        public double TotalNegativaN { get; set; }
        public double ProbabilidadNegativaP { get; set; }
        public double ProbabilidadPositivaN { get; set; }
        public int TotalTuitsPositivos { get; set; }
        public int TotalTuitsNegativos { get; set; }
        public int TuitsPositivosN {get;set;}
        public int TuitsPositivosP {get;set;}
        public int TuitsNegativosN {get;set;}
        public int TuitsNegativosP { get; set; }
    }
}
