namespace ProyectoJuegoDeRol.Models
{
    public class Datos
    {
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public Provincia Provincia { get; set; }

        public void MostrarDatos()
        {
            Console.WriteLine($"Nombre: {Nombre}, Edad: {Edad}, Provincia: {Provincia}");
        }
    }
}