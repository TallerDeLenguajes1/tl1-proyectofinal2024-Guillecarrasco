using ProyectoJuegoDeRol.Models;
using ProyectoJuegoDeRol.Utils;


namespace ProyectoJuegoDeRol.Services
{
    public class FabricaDePersonajes
    {
        private Random random = new Random();

        public async Task<Personaje> CrearPersonajeAleatorioAsync()
        {
            var hobbies = Enum.GetValues(typeof(Hobbie)).Cast<Hobbie>().ToArray();
            var provincias = Enum.GetValues(typeof(Provincia)).Cast<Provincia>().ToArray();
            string nombre = await GeneradorDeNombres.GenerarNombreAsync();

             return new Personaje
            {
                Datos = new Datos
                {
                    Nombre = nombre,
                    Edad = random.Next(18, 60),
                    Provincia = provincias[random.Next(provincias.Length)]
                },
                Caracteristicas = new Caracteristicas
                {
                    Inteligencia = random.Next(1, 11),
                    Atractivo = random.Next(1, 11),
                    Carisma = random.Next(1, 11),
                    Hobbie = hobbies[random.Next(hobbies.Length)]
                },
                Compatibilidad = new Compatibilidad
                {
                    CompatibilidadValor = random.NextDouble()
                }
            };
        }
        public Principe CrearPrincipe()
        {
            var hobbies = Enum.GetValues(typeof(Hobbie)).Cast<Hobbie>().ToArray();
            return new Principe
            {
                Datos = new Datos
                {
                    Nombre = "Príncipe",
                    Edad = random.Next(20, 50),
                    Provincia = Provincia.CiudadAutónomadeBuenosAires 
                },
                Caracteristicas = new Caracteristicas
                {
                    Inteligencia = random.Next(1, 11),
                    Atractivo = random.Next(1, 11),
                    Carisma = random.Next(1, 11),
                    Hobbie = hobbies[random.Next(hobbies.Length)]
                }
            };
        }
    }
}