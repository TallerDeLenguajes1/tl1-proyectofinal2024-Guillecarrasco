namespace ProyectoJuegoDeRol.Models
{
    public class Princesa
    {
        public Datos Datos { get; set; } = new Datos();
        public Caracteristicas Caracteristicas { get; set; } = new Caracteristicas();

        public void MostrarAtributos()
        {
            Datos.MostrarDatos();
            Caracteristicas.MostrarCaracteristicas();
        }
    }
}
