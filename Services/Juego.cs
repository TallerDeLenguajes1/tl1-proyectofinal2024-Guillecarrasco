using ProyectoJuegoDeRol.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ProyectoJuegoDeRol.Services
{
    public class Juego
    {
        private List<Personaje> personajes;
        private FabricaDePersonajes fabrica;
        private Personaje personajePrincipal;
         public Juego()
        {
            fabrica = new FabricaDePersonajes();
            personajes = new List<Personaje>();           
        }

    public async Task IniciarAsync()
        {
        for (int i = 0; i < 20; i++)
        {
            personajes.Add(await fabrica.CrearPersonajeAleatorioAsync());
            }

            MostrarPantallaInicial();
            Console.Clear();
            SeleccionarPersonaje();
        }
    
    private void MostrarPantallaInicial()
        {
            Console.WriteLine("La seleccion");
            Console.Clear();
            Console.ReadKey();
        }

 private void SeleccionarPersonaje()
        {
            Console.Clear();
            Console.WriteLine("Seleccione su personaje:");
            for (int i = 0; i < personajes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {personajes[i].Nombre} (Provincia: {personajes[i].Provincia})");
            }

            int seleccion;
            do
            {
                Console.WriteLine("Ingrese el número del personaje que desea seleccionar:");
                seleccion = int.Parse(Console.ReadLine()) - 1;
            } while (seleccion < 0 || seleccion >= personajes.Count);

            personajePrincipal = personajes[seleccion];
            personajes.RemoveAt(seleccion);

            Jugar();
        }

         private void Jugar()
        {
            Console.Clear();
        }

}


}