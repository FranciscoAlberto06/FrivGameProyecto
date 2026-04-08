using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BGestionFAFA
{
    public class ApiWordle
    {

        // Va a contener el listado de palabras que se va a usar
        private static List<string> listaPalabras;

        // Generador de numeros aleatorios
        private static Random generadorAleatorio = new Random();

        // 1. Limpiamos los acentos usando Replace
        public static string QuitarAcentos(string texto)
        {
            // Lo pasamos a mayusculas y le quitamos los espacios al principio y al final
            string textoLimpio = texto.ToUpper().Trim();

            // Reemplazamos cada vocal acentuada por su equivalente sin acento, y también las vocales con diéresis
            return textoLimpio
                .Replace('Á', 'A')
                .Replace('É', 'E')
                .Replace('Í', 'I')
                .Replace('Ó', 'O')
                .Replace('Ú', 'U')
                .Replace('Ä', 'A')
                .Replace('Ë', 'E')
                .Replace('Ï', 'I')
                .Replace('Ö', 'O')
                .Replace('Ü', 'U');
        }

        // 2. Descargamos y preparamos nuestra única lista (Síncrono)
        public static void InicializarDiccionario()
        {

            // Inciamos el cliente y la URL de donde vamos a sacar el diccionario de palabras, que es un repositorio de GitHub
            HttpClient cliente = new HttpClient();
            string url = "https://raw.githubusercontent.com/javierarce/palabras/master/listado-general.txt";

            try
            {
                // Sacamos todas la palabras
                string todasLasPalabras = cliente.GetStringAsync(url).Result;

                // Incializamos la lista de palabras
                listaPalabras = new List<string>();

                // Separamos el texto en lineas, cada linea es una palabra, y eliminamos las lineas vacías
                string[] lineas = todasLasPalabras.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);// \n es para los saltos de linea,
                                                                                                              // \r es para los saltos de linea en windows que a veces se usan ambos caracteres, y
                                                                                                              // RemoveEmptyEntries es para eliminar las lineas vacías que puedan haber

                // Recorremos todas las palabras para limpiarla y añadirla a la lista solo si tiene entre 4 y 6 letras
                foreach (string palabra in lineas)
                {
                    // Filtramos para que entren a la lista las de 4 a 6 letras
                    if (palabra.Length >= 4 && palabra.Length <= 6)
                    {
                        // Formateamos la palabra adaptándola a nuestro formato 
                        string palabraLimpia = QuitarAcentos(palabra);

                        // Añadimos la palabra a la lista de palabras general
                        listaPalabras.Add(palabraLimpia);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al descargar el diccionario: " + ex.Message);
            }
        }

        // Metodo para extraer una palabra aleatoria
        public static string ObtenerPalabraAleatoria()
        {
            // Cargamos nuestra lista de palabras sacadas de una fuente online 
            InicializarDiccionario();

            // Sacamos un numero aletiorio mediante nuestro generador en el rango del numero de palabras que tenemos en nuestra lista
            int indiceAleatorio = generadorAleatorio.Next(0, listaPalabras.Count);

            // Devolvemos la palabras aletaoria elegida en la lsita
            return listaPalabras[indiceAleatorio];
        }

        // Metodo para comprobar si el intento del usuario es una palabra real
        public static bool ComprobarSiExiste(string intentoPalabraActual)
        {
            
            // Comprobamos si la palabra esta en nuestra lista de palabras validas
            return listaPalabras.Contains(intentoPalabraActual);
        }



        public static string CargarPalabraOffline()
        {
            // Creamos un listado de palabras local para cuando no haya conexion a internet
            listaPalabras = [ "CODIGO", "MAUI", "MOVIL", "JUEGO", "PERRO", "PLATO" ];

            // Seleccionamos una palabra random dentro del rango de opcione que tiene nuestra lista de palabras
            return listaPalabras[generadorAleatorio.Next(listaPalabras.Count)];

        }



    }
}
