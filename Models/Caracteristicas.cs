namespace ProyectoJuegoDeRol.Models
{
    public class Caracteristicas
    {
        public int Carisma { get; set; }
        public int Inteligencia { get; set; }
        public int Atractivo { get; set; }
        public Hobbie Hobbie { get; set; }

        public void MostrarCaracteristicas()
        {
            Console.WriteLine($"Carisma: {Carisma}, Inteligencia: {Inteligencia}, Atractivo: {Atractivo}, Hobbie: {Hobbie}");
        }
    }
}