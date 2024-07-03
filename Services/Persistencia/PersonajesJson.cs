using ProyectoJuegoDeRol.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProyectoJuegoDeRol.Services.Persistencia
{
    public class PersonajesJson
    {
        public void GuardarPersonajes(List<Personaje> personajes, string archivo)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(personajes, options);
            File.WriteAllText(archivo, jsonString);
        }

        public List<Personaje> LeerPersonajes(string archivo)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            string jsonString = File.ReadAllText(archivo);
            return JsonSerializer.Deserialize<List<Personaje>>(jsonString, options);
        }

        public bool Existe(string archivo)
        {
            return File.Exists(archivo) && new FileInfo(archivo).Length > 0;
        }

        public void BorrarDatos(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
        }
    }
}
