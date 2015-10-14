using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naive_Bayes.Models
{
    class Clasificador
    {
        public string tipo { get; set; }
        public Dictionary<string, int> contador { get; set; }
        public int totalPalabras { get; set; }
        public int totalTuits { get; set; }
        private char[] caracteresInvalidos = { '-', '*', '+', '.', ',' };
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
        public void contarPalabras(TuitsList tuits)
        {
            foreach (Tuit tuit in tuits)
            {
                this.totalTuits++;
                foreach (string palabra in tuit.contenido.ToLower().Split(' '))
                {
                    if ((tuit.positivo == "1" && tipo == "positivo") || (tuit.negativo == "1" && tipo == "negativo"))
                    {
                        this.totalPalabras++;
                        string word = this.laContiene(palabra.ToLower().Trim());
                        if (word.Length == 0)
                            continue;
                        word = this.corregirPalabra(word);
                        if (this.contador.ContainsKey(word))
                            this.contador[word]++;
                        else
                        {
                            this.contador.Add(word, 1);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palabra"></param>
        /// <returns></returns>
        private string corregirPalabra(string palabra)
        {
            int toEnd = palabra.Length;
            for (int i = 0; i < toEnd - 1; i++)
            {
                if (palabra[i] == '\'' && i < toEnd - 1)
                {
                    i++;
                    switch (palabra[i])
                    {
                        case 'n': palabra = palabra.Replace("\'n", "ñ"); i = -1; toEnd = palabra.Length; break;
                        case 'a': palabra = palabra.Replace("\'a", "a"); i = -1; toEnd = palabra.Length; break;
                        case 'e': palabra = palabra.Replace("\'e", "e"); i = -1; toEnd = palabra.Length; break;
                        case 'i': palabra = palabra.Replace("\'i", "i"); i = -1; toEnd = palabra.Length; break;
                        case 'o': palabra = palabra.Replace("\'o", "o"); i = -1; toEnd = palabra.Length; break;
                        case 'u': palabra = palabra.Replace("\'u", "u"); i = -1; toEnd = palabra.Length; break;
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
        private string laContiene(string palabra)
        {
            foreach (char caracter in this.caracteresInvalidos)
                if (palabra.Contains(caracter))
                    palabra = palabra.Replace(caracter.ToString(), "");
            return palabra;
        }
    }
}

