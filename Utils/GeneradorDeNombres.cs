using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProyectoJuegoDeRol.Utils
{
    public static class GeneradorDeNombres
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> GenerarNombreAsync(bool gender)
        {
            string apiUrl = gender
            ? "https://namey.muffinlabs.com/name.string?type=female&with_surname=true&frequency=all"
            : "https://namey.muffinlabs.com/name.string?type=male&with_surname=true&frequency=all";
            
            try
            {

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();  


                string nombreSucio = await response.Content.ReadAsStringAsync();

                string nombreLimpio = LimpiarNombre(nombreSucio);
                
                if (!string.IsNullOrWhiteSpace(nombreLimpio))
                {
                    return nombreLimpio;
                }
                else
                {
                    Console.WriteLine("La API devolvió un nombre vacío o inválido.");
                    return "NombreDesconocido";
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return "NombreDesconocido";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el nombre: {ex.Message}");
                return "NombreDesconocido";
            }
        }

        public static string LimpiarNombre(string nombreSucio)
        {
            string nombreLimpio = Regex.Replace(nombreSucio, "<.*?>", string.Empty);
            nombreLimpio = nombreLimpio.Trim();
            return nombreLimpio;
        }

    }
}
