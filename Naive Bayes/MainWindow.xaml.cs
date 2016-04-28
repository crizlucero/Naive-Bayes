using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Naive_Bayes.Models;
using System.IO;
using Excel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Linq;

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
        private int radSelected { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.EntrenarUnigram();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvertir_Click(object sender, RoutedEventArgs e)
        {
            string path = @"C:\archivos\tuits1.arff";
            //string cuerpo = "@relation tuits\n@attribute 'content' string\n@attribute 'pos' numeric\n@attribute 'neg' numeric\n@attribute 'query' string\n@data \n";
            string cuerpo = "@relation tuits\n@attribute 'palabra' string\n@attribute 'cantidad' numeric\n@attribute 'class' {test_positivo, test_negativo}\n@data \n";
            //foreach (Tuit tuit in this.tuits)
            //  cuerpo += "\"" + Clasificador.corregirPalabra(tuit.contenido) + "\"," + tuit.positivo + "," + tuit.negativo + ",\"" + tuit.empresa + "\"\n";
            foreach (KeyValuePair<string, int> palabra in this.clasPositivo.contador)
                cuerpo += "\"" + palabra.Key + "\"," + palabra.Value + ",test_positivo\n";
            foreach (KeyValuePair<string, int> palabra in this.clasNegativo.contador)
                cuerpo += "\"" + palabra.Key + "\"," + palabra.Value + ",test_negativo\n";
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
        private void EntrenarUnigram()
        {
            int count = 0;
            int countP = 0;
            int countN = 0;
            this.clasPositivo.totalPalabras = 0;
            this.clasPositivo.totalTuits = 0;
            this.clasNegativo.totalPalabras = 0;
            this.clasNegativo.totalTuits = 0;
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
            this.clasPositivo.contador.Clear();
            this.clasNegativo.contador.Clear();
            foreach (Tuit tuit in this.tuits)
            {
                //if (count % 512 == 0)
                //{
                if (tuit.positivo == "1" && countP < 150)
                {
                    this.clasPositivo.contarPalabrasUnigram(tuit);
                    countP++;
                }
                if (tuit.negativo == "1" && countN < 150)
                {
                    this.clasNegativo.contarPalabrasUnigram(tuit);
                    countN++;
                }
                //}
                count++;
            }
            this.totalPalabras = this.clasPositivo.totalPalabras + this.clasNegativo.totalPalabras;
            //this.quitarDuplicados();
        }

        private void EntrenarBigram()
        {
            int count = 0;
            int countP = 0;
            int countN = 0;
            this.clasPositivo.totalPalabras = 0;
            this.clasPositivo.totalTuits = 0;
            this.clasNegativo.totalPalabras = 0;
            this.clasNegativo.totalTuits = 0;
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
            this.clasPositivo.contador.Clear();
            this.clasNegativo.contador.Clear();
            foreach (Tuit tuit in this.tuits)
            {
                //if (count % 512 == 0)
                //{
                if (tuit.positivo == "1" && countP < 150)
                {
                    this.clasPositivo.contarPalabrasBigram(tuit);
                    countP++;
                }
                if (tuit.negativo == "1" && countN < 150)
                {
                    this.clasNegativo.contarPalabrasBigram(tuit);
                    countN++;
                }
                //}
                count++;
            }
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
            double ProbabilidadPositivosP = 0;
            double ProbabilidadPositivosN = 0;
            double ProbabilidadNegativosP = 0;
            double ProbabilidadNegativosN = 0;
            int tuitsPositivosP = 0;
            int tuitsPositivosN = 0;
            int tuitsNegativosP = 0;
            int tuitsNegativosN = 0;
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
                        switch (radSelected)
                        {
                            case 0:
                                probabilidadPositivaP += this.ObtenerProbabilidadesNB(this.clasPositivo, tuit.contenido, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                                probabilidadNegativaP += this.ObtenerProbabilidadesNB(this.clasNegativo, tuit.contenido, ref totalNegativosP, ref totalPalabrasPositivas, ref ProbabilidadNegativosP);
                                break;
                            case 1:
                                probabilidadPositivaP += this.ObtenerProbabilidadesLSU(this.clasPositivo, tuit.contenido, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                                probabilidadNegativaP += this.ObtenerProbabilidadesLSU(this.clasNegativo, tuit.contenido, ref totalNegativosP, ref totalPalabrasPositivas, ref ProbabilidadNegativosP);
                                break;
                            case 2:
                                probabilidadPositivaP += this.ObtenerProbabilidadesLSB(this.clasPositivo, tuit.contenido, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                                probabilidadNegativaP += this.ObtenerProbabilidadesLSB(this.clasNegativo, tuit.contenido, ref totalNegativosP, ref totalPalabrasPositivas, ref ProbabilidadNegativosP);
                                break;
                            case 3: break;
                        }
                        if (ProbabilidadPositivosP >= ProbabilidadNegativosP) ++tuitsPositivosP;
                        else ++tuitsNegativosP;
                        totalTuitsPositivos++;
                    }
                    else
                    {//Negativos
                        switch (radSelected)
                        {
                            case 0:
                                probabilidadNegativaN += this.ObtenerProbabilidadesNB(this.clasNegativo, tuit.contenido, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                                probabilidadPositivaN += this.ObtenerProbabilidadesNB(this.clasPositivo, tuit.contenido, ref totalPositivosN, ref totalPalabrasNegativas, ref ProbabilidadPositivosN);
                                break;
                            case 1:
                                probabilidadNegativaN += this.ObtenerProbabilidadesLSU(this.clasNegativo, tuit.contenido, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                                probabilidadPositivaN += this.ObtenerProbabilidadesLSU(this.clasPositivo, tuit.contenido, ref totalPositivosN, ref totalPalabrasNegativas, ref ProbabilidadPositivosN);
                                break;
                            case 2:
                                probabilidadNegativaN += this.ObtenerProbabilidadesLSB(this.clasNegativo, tuit.contenido, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                                probabilidadPositivaN += this.ObtenerProbabilidadesLSB(this.clasPositivo, tuit.contenido, ref totalPositivosN, ref totalPalabrasNegativas, ref ProbabilidadPositivosN);
                                break;
                            case 3: break;
                        }
                        if (ProbabilidadNegativosN >= ProbabilidadPositivosN) ++tuitsNegativosN;
                        else ++tuitsPositivosN;
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
                    TuitsPositivosN = tuitsPositivosN,
                    TuitsPositivosP = tuitsPositivosP,
                    TuitsNegativosN = tuitsNegativosN,
                    TuitsNegativosP = tuitsNegativosP,
                    Duracion = watch.Elapsed
                };
                ResultadoAll win = new ResultadoAll(Resultados);
                win.Show();
            }
            else
            {
                //Dictionary<string, int> Positivos = this.ObtenerValores(this.clasPositivo.contador,this.txtTuit.Text);
                //Dictionary<string, int> Negativos = this.ObtenerValores(this.clasNegativo.contador, this.txtTuit.Text);
                switch (radSelected)
                {
                    case 0:
                        //Positivos
                        probabilidadPositivaP = this.ObtenerProbabilidadesNB(this.clasPositivo, this.txtTuit.Text, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                        //this.lblPositivo.Content = probabilidadPositivaP;
                        //Negativos
                        probabilidadNegativaN = this.ObtenerProbabilidadesNB(this.clasNegativo, this.txtTuit.Text, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                        break;
                    case 1:
                        //Positivos
                        probabilidadPositivaP = this.ObtenerProbabilidadesLSU(this.clasPositivo, this.txtTuit.Text, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                        //this.lblPositivo.Content = probabilidadPositivaP;
                        //Negativos
                        probabilidadNegativaN = this.ObtenerProbabilidadesLSU(this.clasNegativo, this.txtTuit.Text, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                        break;
                    case 2:
                        //Positivos
                        probabilidadPositivaP = this.ObtenerProbabilidadesLSB(this.clasPositivo, this.txtTuit.Text, ref totalPositivosP, ref totalPalabrasPositivas, ref ProbabilidadPositivosP);
                        //this.lblPositivo.Content = probabilidadPositivaP;
                        //Negativos
                        probabilidadNegativaN = this.ObtenerProbabilidadesLSB(this.clasNegativo, this.txtTuit.Text, ref totalNegativosN, ref totalPalabrasNegativas, ref ProbabilidadNegativosN);
                        break;
                    case 3: break;
                }
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
        private double ObtenerProbabilidadesNB(Clasificador clasificador, string Cadena, ref int contadorClasificador, ref int ContadorPalabras, ref double probabilidadTuit)
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
            probabilidadTuit = probabilidad;
            return probabilidad;
        }

        private double ObtenerProbabilidadesLSU(Clasificador clasificador, string Cadena, ref int contadorClasificador, ref int ContadorPalabras, ref double probabilidadTuit)
        {
            double probabilidad = (Convert.ToDouble(clasificador.totalPalabras) / Convert.ToDouble(this.totalPalabras));
            string word = "";
            foreach (string palabra in Cadena.Trim().ToLower().Split(' '))
            {
                ContadorPalabras++;
                word = Clasificador.corregirPalabra(Clasificador.laContiene(palabra.ToLower().Trim()));
                if (clasificador.contador.ContainsKey(word))
                {
                    probabilidad *= Convert.ToDouble(clasificador.contador[word] + 1) * (Convert.ToDouble(clasificador.totalPalabras) / Convert.ToDouble((clasificador.totalPalabras + this.totalPalabras)));
                    contadorClasificador++;
                }
                else
                    probabilidad *= 1.0 / Convert.ToDouble((clasificador.totalPalabras + this.totalPalabras));
            }
            probabilidadTuit = probabilidad;
            return probabilidad;
        }

        private double ObtenerProbabilidadesLSB(Clasificador clasificador, string Cadena, ref int contadorClasificador, ref int ContadorPalabras, ref double probabilidadTuit)
        {
            double probabilidad = (Convert.ToDouble(clasificador.totalPalabras) / Convert.ToDouble(this.totalPalabras));
            string word = "";
            foreach (string palabra in Cadena.Trim().ToLower().Split(' '))
            {
                ContadorPalabras++;
                word = Clasificador.corregirPalabra(Clasificador.laContiene(palabra.ToLower().Trim()));
                if (clasificador.contador.Keys.Contains("|" + word))
                {
                    int suma = 0;
                    foreach (var key in (from keys in clasificador.contador.Keys where keys.Contains(word) select keys).ToList())
                    {
                        suma += clasificador.contador[key];
                    }
                    probabilidad *= (Convert.ToDouble(suma + 1) / Convert.ToDouble(clasificador.totalPalabrasBigram + this.totalPalabras)) * Convert.ToDouble(clasificador.totalPalabrasBigram);
                    contadorClasificador++;

                }
                else
                    probabilidad *= 1.0 / Convert.ToDouble((clasificador.totalPalabras + this.totalPalabras));
            }
            probabilidadTuit = probabilidad;
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

        private void radNB_Checked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "radNB": radSelected = 0; EntrenarUnigram(); break;
                case "radLSU": radSelected = 1; EntrenarUnigram(); break;
                case "radLSB": radSelected = 2; EntrenarBigram(); break;
                case "radSBO": radSelected = 3; break;
            }
        }
    }
}
