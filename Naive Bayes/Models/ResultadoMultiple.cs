using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
