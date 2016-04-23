using System.Collections.Generic;

namespace Naive_Bayes.Models
{
    class Clasificador
    {
        public string tipo { get; set; }
        public Dictionary<string, int> contador { get; set; }
        public int totalPalabras { get; set; }
        public int totalTuits { get; set; }
        public int totalPalabrasBigram { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        public Clasificador(string tipo)
        {
            this.tipo = tipo;
            this.contador = new Dictionary<string, int>();
            this.totalPalabras = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuits"></param>
        public void contarPalabrasUnigram(Tuit tuit)
        {
            //foreach (Tuit tuit in tuits)
            //{
            this.totalTuits++;
            //tuit.contenido = QuitarDeterminantes(tuit.contenido.ToLower());
            this.contador.Clear();
            foreach (string palabra in tuit.contenido.ToLower().Split(' '))
            {
                if ((tuit.positivo == "1" && tipo == "positivo") || (tuit.negativo == "1" && tipo == "negativo"))
                {
                    this.totalPalabras++;
                    string word = laContiene(palabra.Trim());
                    if (word.Length == 0)
                        continue;
                    word = corregirPalabra(word);
                    if (this.contador.ContainsKey(word))
                        this.contador[word]++;
                    else
                        this.contador.Add(word, 1);
                }
            }
            //}
        }

        public void contarPalabrasBigram(Tuit tuit)
        {
            this.totalTuits++;
            this.contador.Clear();
            //tuit.contenido = QuitarDeterminantes(tuit.contenido.ToLower());
            string[] palabra = tuit.contenido.ToLower().Split(' ');
            for (int i = 0; i < palabra.Length - 1; i += 2)
            {
                if ((tuit.positivo == "1" && tipo == "positivo") || (tuit.negativo == "1" && tipo == "negativo"))
                {
                    this.totalPalabras++;
                    string word = laContiene(palabra[i].Trim());
                    if (i != 0 && i != palabra.Length - 1)
                    {
                        word = laContiene(palabra[i - 1].Trim()) + "|" + word;
                    }
                    else if (i == 0)
                    {
                        word = "<inicio>" + word;
                    }
                    else if (i == palabra.Length - 1)
                    {
                        word = word + "<fin>";
                    }
                    if (word.Length == 0)
                        continue;
                    word = corregirPalabra(word);
                    if (this.contador.ContainsKey(word))
                        this.contador[word]++;
                    else
                        this.contador.Add(word, 1);
                }
            }
            totalPalabrasBigram += palabra.Length - 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        public static string corregirPalabra(string palabra)
        {
            QuitarRepeticion(palabra);
            int toEnd = palabra.Length;
            for (int i = 0; i < toEnd - 1; i++)
            {
                if (palabra[i] == '\'' && i < toEnd - 1)
                {
                    i++;
                    switch (palabra[i])
                    {
                        case 'n': palabra = palabra.Replace("\'n", "n"); i = -1; toEnd = palabra.Length; break;
                        case 'a': palabra = palabra.Replace("\'a", "a"); i = -1; toEnd = palabra.Length; break;
                        case 'e': palabra = palabra.Replace("\'e", "e"); i = -1; toEnd = palabra.Length; break;
                        case 'i': palabra = palabra.Replace("\'i", "i"); i = -1; toEnd = palabra.Length; break;
                        case 'o': palabra = palabra.Replace("\'o", "o"); i = -1; toEnd = palabra.Length; break;
                        case 'u': palabra = palabra.Replace("\'u", "u"); i = -1; toEnd = palabra.Length; break;
                    }
                }
                else if (palabra[i] == '\"' && i < toEnd - 1)
                {
                    i++;
                    switch (palabra[i])
                    {
                        case 'a': palabra = palabra.Replace("\"a", "a"); i = -1; toEnd = palabra.Length; break;
                        case 'e': palabra = palabra.Replace("\"e", "e"); i = -1; toEnd = palabra.Length; break;
                        case 'i': palabra = palabra.Replace("\"i", "i"); i = -1; toEnd = palabra.Length; break;
                        case 'o': palabra = palabra.Replace("\"o", "o"); i = -1; toEnd = palabra.Length; break;
                        case 'u': palabra = palabra.Replace("\"u", "u"); i = -1; toEnd = palabra.Length; break;
                    }
                }

            }
            return palabra;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        private string raizPalabra(string palabra)
        {

            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        public static string laContiene(string palabra)
        {
            char[] caracteresInvalidos = { '-', '*', '+', '.', ',', '!', ')', ':', '(', '"', ';', '#', '@' };
            foreach (char caracter in caracteresInvalidos)
                //if (palabra.Contains(caracter))
                palabra = palabra.Replace(caracter.ToString(), "");
            return palabra;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oracion"></param>
        /// <returns></returns>
        public static string QuitarDeterminantes(string oracion)
        {
            string[] Articulos = { " el ", " la ", " los ", " las ", " lo ", " un ", " una ", " unos ", " unas ", " mi ", " su ", " de ", " suyo ", " mio ", " al ", " del " };
            foreach (string articulo in Articulos)
                oracion = oracion.Replace(articulo, " ");
            return oracion;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        public static string QuitarRepeticion(string palabra)
        {
            string nuevaPalabra = "";
            for (int i = 0; i < palabra.Length - 2; i++)
            {
                if (palabra[i] == palabra[i + 1] && (palabra[i + 1] != 'c' || palabra[i + 1] != 'r' || palabra[i + 1] != 'l'))
                {
                    nuevaPalabra += palabra[i + 1];
                    if (palabra[i + 2] == 'c' || palabra[i + 2] == 'r' || palabra[i + 2] == 'l')
                        ++i;
                    ++i;
                }
                else
                    nuevaPalabra += palabra[i];
            }

            return nuevaPalabra;
        }


    }
}

