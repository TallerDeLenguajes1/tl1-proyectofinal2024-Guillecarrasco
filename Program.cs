using ProyectoJuegoDeRol.Services;
using System.Threading.Tasks;
namespace ProyectoJuegoDeRol
{
    class Program
    {
    static async Task Main(string[] args)
    {
        Juego juego = new Juego();
        await juego.IniciarAsync();
    }
    }
}
