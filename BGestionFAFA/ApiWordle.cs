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
        //public static async Task ComprobarSiExiste(string intentoPalabraActual)
        //{
        //    HttpClient cliente = new HttpClient();

        //    // 2. Construimos la URL. 
        //    // Importante: La pasamos a minúsculas ya que las APIs suelen preferirlo así.
        //    string url = $"https://rae-api.com/api/search?word={intentoPalabraActual}";

        //    // 3. Hacemos la petición
        //    HttpResponseMessage respuesta = await cliente.GetAsync(url);
        //}

        public static string ExtraerPalabraRAE()
        {
            // Abrimos nuestro cliente HTTP para hacer la petición a la API de palabras aleatorias
            HttpClient cliente = new HttpClient();

            // Extremos el contenido dado
            string json = cliente.GetStringAsync("https://rae-api.com/api/random?min_length=4&max_length=6").Result; // Esta API devuelve una palabra aleatoria en español con un largo entre 4 y 6 letras

            // Procesamos el JSON
            JsonDocument documentoJSON = JsonDocument.Parse(json);

            // Extraemos la palabra del JSON, navegando por sus propiedades hasta llegar a la palabra que queremos sacar
            string palabraExtraida = documentoJSON.RootElement.GetProperty("data").GetProperty("word").GetString();

            // Devolvemos la palabra extraida
            return palabraExtraida;

        }




    }
}
