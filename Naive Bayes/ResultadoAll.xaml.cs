using Naive_Bayes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.lblTiempoEjecucion.Content = Resultados.Duracion + " Segundos";
        }
    }
}
