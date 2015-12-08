using Naive_Bayes.Models;
using System.Windows;

namespace Naive_Bayes
{
    /// <summary>
    /// Interaction logic for ResultadoSingle.xaml
    /// </summary>
    public partial class ResultadoSingle : Window
    {
        public ResultadoSingle(ResultadoSencillo res)
        {
            InitializeComponent();
            CargarComponentes(res);
        }
        private void CargarComponentes(ResultadoSencillo res)
        {
            this.lblTotalPalabrasPositivas.Content = res.TotalPalabrasPositivas;
            this.lblTotalPalabrasNegativas.Content = res.TotalPalabrasNegativas;
            this.lblPP.Content = res.ProbabilidadPositivaP;
            this.lblPN.Content = res.ProbabilidadNegativaN;
            this.lblTipo.Content = "Tuit " + (string)(res.ProbabilidadPositivaP > res.ProbabilidadNegativaN ? "Positivo" : "Negativo");
            this.lblTiempoEjecucion.Content = res.Duracion;
        }
    }
}
