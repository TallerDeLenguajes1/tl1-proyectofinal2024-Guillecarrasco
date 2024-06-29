using System;

namespace ProyectoJuegoDeRol.Models
{
    public class Principe
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Carisma { get; set; }
        public int Inteligencia { get; set; }
        public int Atractivo { get; set; }
        public Hobbie Hobbie { get; set; }


        public void MostrarAtributos()
        {
            Console.WriteLine($"Nombre: {Nombre}, Edad: {Edad}, Carisma: {Carisma}, Inteligencia: {Inteligencia}, Atractivo: {Atractivo}, Hobbie: {Hobbie}");
        }
    }
}
