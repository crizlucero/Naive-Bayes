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
using System.IO;
using Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
                cuerpo += "\"" + Clasificador.corregirPalabra(tuit.contenido) + "\"," + tuit.positivo + "," + tuit.negativo + ",\"" + tuit.empresa + "\"\n";

            if (!File.Exists(path))
                File.Create(path).Dispose();
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
            int countP = 0;
            int countN = 0;
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
                    if (tuit.positivo == "1" && countP < 50)
                    {
                        this.clasPositivo.contarPalabras(tuit);
                        countP++;
                    }
                    if (tuit.negativo == "1" && countN < 50)
                    {
                        this.clasNegativo.contarPalabras(tuit);
                        countN++;
                    }
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
            double probabilidadPositivaP = 0.0;
            double probabilidadPositivaN = 0.0;
            double probabilidadNegativaP = 0.0;
            double probabilidadNegativaN = 0.0;
            int totalPositivosP = 0;
            int totalNegativosP = 0;
            int totalPositivosN = 0;
            int totalNegativosN = 0;
            int totalTuitsPositivos = 0;
            int totalTuitsNegativos = 0;
            int totalPalabrasPositivas = 0;
            int totalPalabrasNegativas = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (this.radAll.IsChecked.Value)
            {

                foreach (Tuit tuit in this.tuits)
                {
                    //Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador, tuit.contenido);
                    //Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador, tuit.contenido);

                    //Positivos
                    if (tuit.positivo == "1")
                    {
                        probabilidadPositivaP += this.ObtenerProbabilidades(this.clasPositivo, tuit.contenido, ref totalPositivosP, ref totalPalabrasPositivas);
                        probabilidadNegativaP += this.ObtenerProbabilidades(this.clasNegativo, tuit.contenido, ref totalNegativosP, ref totalPalabrasPositivas);
                        totalTuitsPositivos++;
                    }
                    else
                    {//Negativos
                        probabilidadNegativaN += this.ObtenerProbabilidades(this.clasNegativo, tuit.contenido, ref totalNegativosN, ref totalPalabrasNegativas);
                        probabilidadPositivaN += this.ObtenerProbabilidades(this.clasPositivo, tuit.contenido, ref totalPositivosN, ref totalPalabrasNegativas);
                        totalTuitsNegativos++;
                    }
                }

                watch.Stop();
                ResultadoMultiple Resultados = new ResultadoMultiple()
                {
                    TotalTuits = this.tuits.Count,
                    ProbabilidadPositivaP = probabilidadPositivaP,
                    ProbabilidadNegativaP = probabilidadNegativaP,
                    TotalPositivaP = totalPositivosP,
                    TotalNegativaP = totalNegativosP,
                    ProbabilidadNegativaN = probabilidadNegativaN,
                    ProbabilidadPositivaN = probabilidadPositivaN,
                    TotalNegativaN = totalNegativosN,
                    TotalPositivaN = totalPositivosN,
                    TotalPalabrasPositivas = totalPalabrasPositivas / 2,
                    TotalPalabrasNegativas = totalPalabrasNegativas / 2,
                    TotalTuitsPositivos = totalTuitsPositivos,
                    TotalTuitsNegativos = totalTuitsNegativos,
                    Duracion = watch.Elapsed
                };
                ResultadoAll win = new ResultadoAll(Resultados);
                win.Show();
            }
            else
            {
                //Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador,this.txtTuit.Text);
                //Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador, this.txtTuit.Text);

                //Positivos
                probabilidadPositivaP = this.ObtenerProbabilidades(this.clasPositivo, this.txtTuit.Text, ref totalPositivosP, ref totalPalabrasPositivas);
                //this.lblPositivo.Content = probabilidadPositivaP;
                //Negativos
                probabilidadNegativaN = this.ObtenerProbabilidades(this.clasNegativo, this.txtTuit.Text, ref totalNegativosN, ref totalPalabrasNegativas);
                watch.Stop();
                /*this.lblNegativo.Content = probabilidadNegativaN;
                this.lblSeleccion.Content = probabilidadPositivaP > probabilidadNegativaN ? "Positivo" : "Negativo";*/
                ResultadoSencillo res = new ResultadoSencillo()
                {
                    TotalPalabrasPositivas = totalPalabrasPositivas,
                    TotalPalabrasNegativas = totalPalabrasNegativas,
                    ProbabilidadPositivaP = probabilidadPositivaP,
                    ProbabilidadNegativaN = probabilidadNegativaN,
                    Duracion = watch.Elapsed
                };
                ResultadoSingle win = new ResultadoSingle(res);
                win.Show();
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
                if (this.ContienePalabra(Cadena, palabra))
                    if (valores.ContainsKey(palabra))
                        valores[palabra]++;
                    else
                        valores.Add(palabra, 1);
            return valores;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clasificador"></param>
        /// <param name="Cadena"></param>
        /// <param name="contadorClasificador"></param>
        /// <param name="ContadorPalabras"></param>
        /// <returns></returns>
        private double ObtenerProbabilidades(Clasificador clasificador, string Cadena, ref int contadorClasificador, ref int ContadorPalabras)
        {
            double probabilidad = (Convert.ToDouble(clasificador.totalPalabras) / Convert.ToDouble(this.totalPalabras));
            string word = "";
            foreach (string palabra in Cadena.Trim().ToLower().Split(' '))
            {
                ContadorPalabras++;
                word = Clasificador.corregirPalabra(Clasificador.laContiene(palabra.ToLower().Trim()));
                if (clasificador.contador.ContainsKey(word))
                {
                    probabilidad *= Convert.ToDouble((clasificador.contador[word] + 1)) / Convert.ToDouble((clasificador.totalPalabras + this.totalPalabras));
                    contadorClasificador++;
                }
                else
                    probabilidad *= 1.0 / Convert.ToDouble((clasificador.totalPalabras + this.totalPalabras));
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
                if (frag == palabra)
                    return true;
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

        private void txtTuit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btnClasificar_Click(null, null);
            }
        }
    }
}
