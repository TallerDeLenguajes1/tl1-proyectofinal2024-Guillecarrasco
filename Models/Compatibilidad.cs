namespace ProyectoJuegoDeRol.Models
{
    public class Compatibilidad
    {
        public double CompatibilidadValor { get; set; }

        public void MostrarCompatibilidad()
        {
            Console.WriteLine($"Compatibilidad: {CompatibilidadValor}");
        }
    }
}
