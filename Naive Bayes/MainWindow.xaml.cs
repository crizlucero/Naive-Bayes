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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Naive_Bayes.Models;
using CsvHelper;
using System.IO;
using Excel;

namespace Naive_Bayes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TuitsList tuits = new TuitsList();
        private Clasificador clasPositivo = new Clasificador("positivo");
        private Clasificador clasNegativo = new Clasificador("negativo");
        private int totalPalabras { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEntrenar_Click(object sender, RoutedEventArgs e)
        {
            this.tuits.Clear();
            //var reader = new StreamReader(File.OpenRead(@"C:\archivos\twitters-spanish-12k.csv"));
            foreach (var ws in Workbook.Worksheets(@"C:\archivos\twitters-spanish-12k.xlsx"))
            {
                for (int i = 1; i < ws.Rows.Length; i++)
                {
                    this.tuits.Add(new Tuit()
                    {
                        contenido = ws.Rows[i].Cells[0].Text,
                        positivo = ws.Rows[i].Cells[1].Text,
                        negativo = ws.Rows[i].Cells[2].Text,
                        empresa = ws.Rows[i].Cells[3].Text
                    });
                }
            }
            this.clasPositivo.contarPalabras(this.tuits);
            this.clasNegativo.contarPalabras(this.tuits);
            this.totalPalabras = this.clasPositivo.totalPalabras + this.clasNegativo.totalPalabras + 1;
            //this.quitarDupicados();
        }

        private void quitarDupicados()
        {
            Dictionary<string, int> auxP = new Dictionary<string, int>(this.clasPositivo.contador);
            foreach (string val in auxP.Keys)
                if (this.clasNegativo.contador.Remove(val))
                    this.clasPositivo.contador.Remove(val);
            this.clasPositivo.totalPalabras = this.clasPositivo.contador.Count + this.clasNegativo.contador.Count;
            this.clasNegativo.totalPalabras = this.clasPositivo.totalPalabras;
        }

        private void btnClasificar_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador);
            Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador);

            //Positivos
            double probabilidadPositiva = this.ObtenerProbabilidades(this.clasPositivo);//, Positivos);
            this.lblPositivo.Content = probabilidadPositiva;
            //Negativos
            double probabilidadNegativa = this.ObtenerProbabilidades(this.clasNegativo);//, Negativos);
            this.lblNegativo.Content = probabilidadNegativa;
            this.lblSeleccion.Content = probabilidadPositiva > probabilidadNegativa ? "Positivo" : "Negativo";
        }
        private Dictionary<string, int> ObtenerValores(Dictionary<string, int> clasificacion)
        {
            Dictionary<string, int> valores = new Dictionary<string, int>();
            //Positivos
            foreach (string palabra in clasificacion.Keys)
            {
                if (this.ContienePalabra(this.txtTuit.Text, palabra))
                {
                    if (valores.ContainsKey(palabra))
                    {
                        valores[palabra]++;
                    }
                    else
                    {
                        valores.Add(palabra, 1);
                    }
                }
            }
            return valores;
        }

        private double ObtenerProbabilidades(Clasificador clas)
        {
            double probabilidad = (Convert.ToDouble(clas.totalPalabras) / Convert.ToDouble(this.totalPalabras));
            foreach (string palabra in this.txtTuit.Text.Split(' '))
            {
                if (clas.contador.ContainsKey(palabra))
                    probabilidad *= Convert.ToDouble((clas.contador[palabra] + 1)) / Convert.ToDouble((clas.totalPalabras + this.totalPalabras));
                else
                    probabilidad *= 1.0 / Convert.ToDouble((clas.totalPalabras + this.totalPalabras));
            }
            return probabilidad;
        }

        private bool ContienePalabra(string texto, string palabra)
        {
            foreach (string frag in texto.Split(' '))
            {
                if (frag == palabra)
                    return true;
            }
            return false;
        }
    }
}
