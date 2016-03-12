using Microsoft.AspNet.Mvc;
using NaiveBayesWeb.Models;
using Excel;

namespace NaiveBayesWeb.Controllers
{
    public class HomeController : Controller
    {
        private TuitList tuits = new TuitList();
        private Clasificador clasPositivo = new Clasificador("positivo");
        private Clasificador clasNegativo = new Clasificador("negativo");
        private int totalPalabras { get; set; }

        public HomeController()
        {
            this.Entrenar();
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
                if (count % 1024 == 0)
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
