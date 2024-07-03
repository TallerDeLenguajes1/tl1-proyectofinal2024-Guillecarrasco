using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProyectoJuegoDeRol.Utils
{
    public static class GeneradorDeNombres
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> GenerarNombreAsync()
        {
            string apiUrl = "https://namey.muffinlabs.com/name.string?type=female&with_surname=true&frequency=all";

            try
            {

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();  


                string nombre = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    return nombre;
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
    }
}
