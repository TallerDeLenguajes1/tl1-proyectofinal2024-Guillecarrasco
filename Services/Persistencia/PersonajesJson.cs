using ProyectoJuegoDeRol.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;

namespace ProyectoJuegoDeRol.Services.Persistencia
{
    public class PersonajesJson
    {
        public void GuardarPersonajes(List<Personaje> personajes, string archivo)
        {
            File.WriteAllText(archivo, JsonConvert.SerializeObject(personajes, new StringEnumConverter()));
        }

        public List<Personaje> LeerPersonajes(string archivo)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.DeserializeObject<List<Personaje>>(File.ReadAllText(archivo), settings);
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
