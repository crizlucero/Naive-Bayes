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
using System.Diagnostics;

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
            this.Entrenar();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvertir_Click(object sender, RoutedEventArgs e)
        {
            string path = @"C:\archivos\tuits.arff";
            string cuerpo = "@relation tuits\n@attribute 'content' string\n@attribute 'pos' numeric\n@attribute 'neg' numeric\n@attribute 'query' string\n@data \n";
            foreach (Tuit tuit in this.tuits)
            {
                cuerpo += "\"" + Clasificador.corregirPalabra(tuit.contenido) + "\"," + tuit.positivo + "," + tuit.negativo + ",\"" + tuit.empresa + "\"\n";
            }
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            using (TextWriter tw = new StreamWriter(path))
            {
                tw.Write(cuerpo);
                tw.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Entrenar()
        {
            int count = 0;
            //var reader = new StreamReader(File.OpenRead(@"C:\archivos\twitters-spanish-12k.csv"));
            foreach (var ws in Workbook.Worksheets(@"C:\archivos\twitters-spanish-12k.xlsx"))
                for (int i = 1; i < ws.Rows.Length; i++)
                    this.tuits.Add(new Tuit()
                    {
                        contenido = ws.Rows[i].Cells[0].Text,
                        positivo = ws.Rows[i].Cells[1].Text,
                        negativo = ws.Rows[i].Cells[2].Text,
                        empresa = ws.Rows[i].Cells[3].Text
                    });
            foreach (Tuit tuit in this.tuits)
            {
                if (count % 13 == 0)
                {
                    this.clasPositivo.contarPalabras(tuit);
                    this.clasNegativo.contarPalabras(tuit);
                }
                count++;
            }
            //this.clasPositivo.contarPalabras(this.tuits);
            //this.clasNegativo.contarPalabras(this.tuits);
            this.totalPalabras = this.clasPositivo.totalPalabras + this.clasNegativo.totalPalabras;
            //this.quitarDuplicados();
        }
        /// <summary>
        /// 
        /// </summary>
        private void quitarDuplicados()
        {
            Dictionary<string, int> auxP = new Dictionary<string, int>(this.clasPositivo.contador);
            foreach (string val in auxP.Keys)
                if (this.clasNegativo.contador.Remove(val))
                    this.clasPositivo.contador.Remove(val);
            this.clasPositivo.totalPalabras = this.clasPositivo.contador.Count + this.clasNegativo.contador.Count;
            this.clasNegativo.totalPalabras = this.clasPositivo.totalPalabras;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClasificar_Click(object sender, RoutedEventArgs e)
        {
            if (this.radAll.IsChecked.Value)
            {
                double probabilidadPositiva = 0.0;
                double probabilidadNegativa = 0.0;
                int totalPositivos = 0;
                int totalNegativos = 0;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (Tuit tuit in this.tuits)
                {
                    //Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador, tuit.contenido);
                    //Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador, tuit.contenido);

                    //Positivos
                    if (tuit.positivo == "1")
                    {
                        probabilidadPositiva += this.ObtenerProbabilidades(this.clasPositivo, tuit.contenido);
                        totalPositivos++;
                    }
                    else
                    {//Negativos
                        probabilidadNegativa += this.ObtenerProbabilidades(this.clasNegativo, tuit.contenido);
                        totalNegativos++;
                    }
                }

                watch.Stop();
                string[] Resultados = { this.tuits.Count.ToString(), totalPositivos.ToString(), probabilidadPositiva.ToString(), totalNegativos.ToString(), probabilidadNegativa.ToString(), watch.Elapsed.ToString() };
                ResultadoAll win = new ResultadoAll(Resultados);
                win.Show();
            }
            else
            {
                //Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador,this.txtTuit.Text);
                //Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador, this.txtTuit.Text);

                //Positivos
                double probabilidadPositiva = this.ObtenerProbabilidades(this.clasPositivo, this.txtTuit.Text);
                this.lblPositivo.Content = probabilidadPositiva;
                //Negativos
                double probabilidadNegativa = this.ObtenerProbabilidades(this.clasNegativo, this.txtTuit.Text);
                this.lblNegativo.Content = probabilidadNegativa;
                this.lblSeleccion.Content = probabilidadPositiva > probabilidadNegativa ? "Positivo" : "Negativo";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clasificacion"></param>
        /// <param name="Cadena"></param>
        /// <returns></returns>
        private Dictionary<string, int> ObtenerValores(Dictionary<string, int> clasificacion, string Cadena)
        {
            Dictionary<string, int> valores = new Dictionary<string, int>();
            //Positivos
            foreach (string palabra in clasificacion.Keys)
            {
                if (this.ContienePalabra(Cadena, palabra))
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clas"></param>
        /// <returns></returns>
        private double ObtenerProbabilidades(Clasificador clas, string Cadena)
        {
            double probabilidad = (Convert.ToDouble(clas.totalPalabras) / Convert.ToDouble(this.totalPalabras));
            string word = "";
            foreach (string palabra in Cadena.Trim().ToLower().Split(' '))
            {
                word = Clasificador.corregirPalabra(Clasificador.laContiene(palabra.ToLower().Trim()));
                if (clas.contador.ContainsKey(word))
                    probabilidad *= Convert.ToDouble((clas.contador[word] + 1)) / Convert.ToDouble((clas.totalPalabras + this.totalPalabras));
                else
                    probabilidad *= 1.0 / Convert.ToDouble((clas.totalPalabras + this.totalPalabras));
            }
            return probabilidad;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="palabra"></param>
        /// <returns></returns>
        private bool ContienePalabra(string texto, string palabra)
        {
            foreach (string frag in texto.Split(' '))
            {
                if (frag == palabra)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radSingle_Checked(object sender, RoutedEventArgs e)
        {
            this.txtTuit.IsEnabled = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtTuit.IsEnabled = false;
            }
            catch { }
        }
    }
}
