using Naive_Bayes.Models;

namespace Naive_Bayes
{
    /// <summary>
    /// Interaction logic for ResultadoAll.xaml
    /// </summary>
    public partial class ResultadoAll
    {

        public ResultadoAll(ResultadoMultiple Resultados)
        {
            InitializeComponent();
            this.CargarDatos(Resultados);
        }

        private void CargarDatos(ResultadoMultiple Resultados)
        {
            this.lblTotal.Content = Resultados.TotalTuits;
            this.lblTotalTuitsPositivos.Content = Resultados.TotalTuitsPositivos;
            this.lblTotalPalabrasPositivas.Content = Resultados.TotalPalabrasPositivas;
            this.lblTotalPalabrasNegativas.Content = Resultados.TotalPalabrasNegativas;
            this.lblTotalTuitsNegativos.Content = Resultados.TotalTuitsNegativos;
            this.lblPP.Content = Resultados.ProbabilidadPositivaP;
            this.lblPN.Content = Resultados.ProbabilidadPositivaN;
            this.lblTotalPP.Content = Resultados.TotalPositivaP;
            this.lblTotalPN.Content = Resultados.TotalPositivaN;
            this.lblNN.Content = Resultados.ProbabilidadNegativaN;
            this.lblNP.Content = Resultados.ProbabilidadNegativaP;
            this.lblTotalNP.Content = Resultados.TotalNegativaP;
            this.lblTotalNN.Content = Resultados.TotalNegativaN;
            this.lblPPPercent.Content = (((Resultados.TuitsPositivosP*1.0) / (Resultados.TotalTuitsPositivos*1.0))*100.0).ToString("#.##") + " %";
            this.lblNPPercent.Content = (((Resultados.TuitsNegativosP*1.0) / (Resultados.TotalTuitsPositivos*1.0))*100.0).ToString("#.##") + " %";
            this.lblPNPercent.Content = (((Resultados.TuitsPositivosN*1.0) / (Resultados.TotalTuitsNegativos*1.0))*100.0).ToString("#.##") + " %";
            this.lblNNPercent.Content = (((Resultados.TuitsNegativosN*1.0) / (Resultados.TotalTuitsNegativos*1.0))*100.0).ToString("#.##") + " %";
            this.lblTotalPN.Content = Resultados.TuitsPositivosN;
            this.lblTotalPP.Content = Resultados.TuitsPositivosP;
            this.lblTotalNN.Content = Resultados.TuitsNegativosN;
            this.lblTotalNP.Content = Resultados.TuitsNegativosP;
            this.lblTiempoEjecucion.Content = Resultados.Duracion + " Segundos";
        }
    }
}
