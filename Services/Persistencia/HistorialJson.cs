using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ProyectoJuegoDeRol.Services.Persistencia
{
    public class HistorialJson
    {
        public void GuardarHistorial(List<string> historial, string archivo)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(historial, options);
            File.WriteAllText(archivo, jsonString);
        }

        public List<string> LeerHistorial(string archivo)
        {
            if (File.Exists(archivo) && new FileInfo(archivo).Length > 0)
            {
                string jsonString = File.ReadAllText(archivo);
                return JsonSerializer.Deserialize<List<string>>(jsonString);
            }
            return new List<string>();
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
