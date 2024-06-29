using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ProyectoJuegoDeRol.Services.Persistencia
{
    public class HistorialJson
    {
        public void GuardarHistorial(List<string> historial, string archivo)
        {
            File.WriteAllText(archivo, JsonConvert.SerializeObject(historial));
        }

        public List<string> LeerHistorial(string archivo)
        {
            if (File.Exists(archivo) && new FileInfo(archivo).Length > 0)
            {
                return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(archivo));
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
