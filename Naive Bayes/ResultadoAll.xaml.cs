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
        public ResultadoAll(string[] Resultados)
        {
            InitializeComponent();
            this.CargarDatos(Resultados);
        }

        private void CargarDatos(string[] Resultados)
        {
            this.lblTotal.Content = Resultados[0];
            this.lblPP.Content = Resultados[1];
            this.lblPN.Content = Resultados[2];
            this.lblNN.Content = Resultados[3];
            this.lblNP.Content = Resultados[4];
            /*this.lblTotalPositivos.Content = Resultados[1];
            this.lblProbabilidadPositivos.Content = Resultados[2];
            this.lblTotalNegativos.Content = Resultados[3];
            this.lblProbabilidadNegativos.Content = Resultados[4];*/
            this.lblTiempoEjecucion.Content = Resultados[5] + " Segundos";
        }
    }
}
