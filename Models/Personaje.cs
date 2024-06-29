using System;
namespace ProyectoJuegoDeRol.Models
{
    public class Personaje
    {
        public string Nombre { get; set; }=string.Empty;
        public int Edad { get; set; }
        public Provincia Provincia { get; set; }
        public int Carisma { get; set; }
        public int Inteligencia { get; set; }
        public int Atractivo { get; set; }
        public Hobbie Hobbie { get; set; }
        
        public void MostrarAtributos()
        {
            Console.WriteLine($"Nombre: {Nombre}, Edad: {Edad}, Provincia: {Provincia}, Carisma: {Carisma}, Inteligencia: {Inteligencia}, Atractivo: {Atractivo}, Hobbie: {Hobbie}");
        }
    }
}
