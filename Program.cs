using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace decodificador
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Inicializamos algunas variables
            List<string> listaATarducir = new List<string>();
            List<int[]> listaAgrupador = new List<int[]>();
            int cont1 = 0, cont0 = 0;
            string frase = "";


            StreamReader archivoCodificado = new StreamReader("archivo.txt"); // Tomamos el código encriptado
            string fraseCodificada = archivoCodificado.ReadToEnd();

            string caracterCambiado = CambiarCaracter(fraseCodificada);
            cont1 = ContarUnos(caracterCambiado); //Sabemos la cantidad de unos

            cont0 = 8 - cont1;

            listaATarducir.Add(caracterCambiado);

            //Dividimos la cadena desde la posición 8 en adelante
            string restanteCodigoCortado = fraseCodificada.ToString().Substring(8);


            // Convertimos de caracteres a enteros
            for (int i = 1; i <= restanteCodigoCortado.Length / 8; i++)
            {
                string cadenaCaracter = Agrupador(restanteCodigoCortado, i - 1, 8);
                char[] freaseToCharArray = cadenaCaracter.ToCharArray();
                int[] enteros = freaseToCharArray.Select(c => (int)Char.GetNumericValue(c)).ToArray();
                listaAgrupador.Add(enteros);
            }

            for (int i = 0; i < listaAgrupador.Count; i++)
            {
                var decodeBinary = Decodificador(cont0, cont1, listaAgrupador[i]);
                string binaryFrase = "";
                for (int x = 0; x < 8; x++)
                    binaryFrase += decodeBinary[x];

                //Evaluamos nuevamente la cantidad de 0 o de 1 para poder aplicarlo la regla de codificación a la próxima letra
                cont1 = ContarUnos(binaryFrase);
                cont0 = 8 - cont1;

                string fragmentoFrase = "";
                for (int j = 0; j < 8; j++)
                    fragmentoFrase += decodeBinary[j];

                listaATarducir.Add(fragmentoFrase);

            }

            string texto = Traductor(listaATarducir);
            Console.WriteLine(texto);
            Console.ReadKey();



        }


        //Esta función es la encargada de fraccionar el código para que posteriormente logremos hacer el proceso de decodificación
        public static string Agrupador(string fraseBinary, int posicion, int contador)
        {
            string fragmento = fraseBinary.ToString().Substring(posicion * contador, 8);
            return fragmento;
        }
        public static string Traductor(List<string> binaryList)
        {
            string binaryString = string.Join("", binaryList);
            byte[] bytes = new byte[binaryString.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                string binaryByte = binaryString.Substring(i * 8, 8);
                bytes[i] = Convert.ToByte(binaryByte, 2);
            }
            return System.Text.Encoding.ASCII.GetString(bytes);
        }


        public static string CambiarCaracter(string fraseCodificada)
        {
            string char1 = fraseCodificada.Substring(0, 8);
            string modificada = char1.Replace("0", "2");
            modificada = modificada.Replace("1", "0");
            modificada = modificada.Replace("2", "1");

            return modificada;
        }

        //Esta es la función encargada de realizar la decodificación del código
        public static int[] Decodificador(int cont0, int cont1, int[] vector)
        {
            int offset = 0;
            // Evaluar la cantidad de 1s y de 0s
            if (cont1 > cont0)
                offset = 1;
            else if (cont0 > cont1)
                offset = 2;
            else
                offset = 3;

            //Cambia los valores de 0 a 1 o viceversa
            for (int i = offset - 1; i < 8; i += offset)
            {
                if (vector[i] == 1)
                    vector[i] = 0;
                else
                    vector[i] = 1;
            }

            return vector;
        }

        //Evalua la cantidad de 0 o de 1 en una cadena de caracteres
        public static int ContarUnos(string frase)
        {
            string codigoCortado = frase.Substring(0, 8);
            List<int> vector = new List<int>();

            for (int i = 0; i < codigoCortado.Length; i++)
                vector.Add(codigoCortado[i]);

            int cont = 0;

            for (int i = 0; i < 8; i++)
                if (vector[i] == 49)
                    cont++;


            return cont;

        }
    }
}
