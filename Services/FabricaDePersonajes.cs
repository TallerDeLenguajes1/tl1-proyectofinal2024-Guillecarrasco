using ProyectoJuegoDeRol.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoJuegoDeRol.Services
{
    public class FabricaDePersonajes
    {
        private Random random = new Random();

        public async Task<Personaje> CrearPersonajeAleatorioAsync()
        {
            var hobbies = Enum.GetValues(typeof(Hobbie)).Cast<Hobbie>().ToArray();
            var Provincias = Enum.GetValues(typeof(Provincia)).Cast<Provincia>().ToArray();
            string nombre ="Desconocido";

            return new Personaje
            {
                Nombre = nombre,
                Provincia = Provincias[random.Next(Provincias.Length)],
                Inteligencia = random.Next(1, 11),
                Atractivo = random.Next(1, 11),
                Carisma = random.Next(1, 11),
                Hobbie = hobbies[random.Next(hobbies.Length)]
            };
        }


    }
}