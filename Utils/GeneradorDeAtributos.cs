using ProyectoJuegoDeRol.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace ProyectoJuegoDeRol.Utils
{

    public static class GeneradorDeAtributos
    {
public static bool RealizarAccion(Personaje personaje, int accion)
{
    bool accionValida = true;

    switch (accion)
    {
        case 1:
            if (personaje.Inteligencia < 10)
                personaje.Inteligencia++;
            else
            {
                Console.WriteLine("No puedes subir más porque ya tienes la inteligencia al máximo.");
                accionValida = false;
            }
            break;
        case 2:
            if (personaje.Inteligencia > 0)
                personaje.Inteligencia--;
            else
            {
                Console.WriteLine("No puedes bajar más porque ya tienes la inteligencia al mínimo.");
                accionValida = false;
            }
            break;
        case 3:
            if (personaje.Atractivo < 10)
                personaje.Atractivo++;
            else
            {
                Console.WriteLine("No puedes subir más porque ya tienes el atractivo al máximo.");
                accionValida = false;
            }
            break;
        case 4:
            if (personaje.Atractivo > 0)
                personaje.Atractivo--;
            else
            {
                Console.WriteLine("No puedes bajar más porque ya tienes el atractivo al mínimo.");
                accionValida = false;
            }
            break;
        case 5:
            if (personaje.Carisma < 10)
                personaje.Carisma++;
            else
            {
                Console.WriteLine("No puedes subir más porque ya tienes el carisma al máximo.");
                accionValida = false;
            }
            break;
        case 6:
            if (personaje.Carisma > 0)
                personaje.Carisma--;
            else
            {
                Console.WriteLine("No puedes bajar más porque ya tienes el carisma al mínimo.");
                accionValida = false;
            }
            break;
        case 7:
            var hobbies = Enum.GetValues(typeof(Hobbie));
            personaje.Hobbie = (Hobbie)hobbies.GetValue(new Random().Next(hobbies.Length));
            break;
        default:
            Console.WriteLine("Acción no válida");
            accionValida = false;
            break;
    }

    if (accionValida)
    {
        Console.WriteLine("Acción realizada con éxito.");
        return true;
    }
    else
    {
        Console.WriteLine("Inténtalo de nuevo.");
        return false;
    }
}
public static bool RealizarAccion2(Personaje personaje, int accion)
{
    bool accionValida = true;

    switch (accion)
    {
        case 1:
            if (personaje.Inteligencia < 10)
                personaje.Inteligencia++;
            else
            {
                accionValida = false;
            }
            break;
        case 2:
            if (personaje.Inteligencia > 0)
                personaje.Inteligencia--;
            else
            {
                accionValida = false;
            }
            break;
        case 3:
            if (personaje.Atractivo < 10)
                personaje.Atractivo++;
            else
            {
                accionValida = false;
            }
            break;
        case 4:
            if (personaje.Atractivo > 0)
                personaje.Atractivo--;
            else
            {
                accionValida = false;
            }
            break;
        case 5:
            if (personaje.Carisma < 10)
                personaje.Carisma++;
            else
            {
                accionValida = false;
            }
            break;
        case 6:
            if (personaje.Carisma > 0)
                personaje.Carisma--;
            else
            {
                accionValida = false;
            }
            break;
        case 7:
            var hobbies = Enum.GetValues(typeof(Hobbie));
            personaje.Hobbie = (Hobbie)hobbies.GetValue(new Random().Next(hobbies.Length));
            break;
        default:
            accionValida = false;
            break;
    }

    if (accionValida)
    {
        return true;
    }
    else
    {
        return false;
    }
}

    }

public static class GeneradorDeNombres
{
    private static readonly HttpClient httpClient = new HttpClient();

   public static async Task<string> GenerarNombreAsync()
{
    try
    {
        string apiUrl = "https://namey.muffinlabs.com/name.string?type=female";

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
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
            else
            {
                Console.WriteLine($"Error al obtener el nombre. Código de estado: {response.StatusCode}");
                return "NombreDesconocido";
            }
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


