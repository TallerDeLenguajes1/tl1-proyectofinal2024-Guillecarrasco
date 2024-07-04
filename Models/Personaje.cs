namespace ProyectoJuegoDeRol.Models
{
    public class Personaje
    {
        public Datos Datos { get; set; } = new Datos();
        public Caracteristicas Caracteristicas { get; set; } = new Caracteristicas();
        public Compatibilidad Compatibilidad { get; set; } = new Compatibilidad();
    }
}
