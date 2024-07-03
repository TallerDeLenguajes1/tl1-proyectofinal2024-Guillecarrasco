using ProyectoJuegoDeRol.Models;
using ProyectoJuegoDeRol.Services.Persistencia;
using ProyectoJuegoDeRol.Utils;
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
        private Principe principe;
        private PersonajesJson personajesJson;
        private HistorialJson historialJson;
        private List<string> historial;
        private Dictionary<Personaje, double> compatibilidadAnterior;
        public Juego()
        {
            fabrica = new FabricaDePersonajes();
            personajes = new List<Personaje>();
            personajesJson = new PersonajesJson();
            historialJson = new HistorialJson();    
            compatibilidadAnterior = new Dictionary<Personaje, double>();     
        }

    public async Task IniciarAsync()
        {
             if (personajesJson.Existe("Data/personajes.json"))
            {
                personajes = personajesJson.LeerPersonajes("Data/personajes.json");
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    personajes.Add(await fabrica.CrearPersonajeAleatorioAsync());
                }
                personajesJson.GuardarPersonajes(personajes, "Data/personajes.json");
            }
            
            principe = fabrica.CrearPrincipe();

            foreach (var personaje in personajes)
            {
                double compatibilidadInicial = CalcularCompatibilidad(personaje);
                compatibilidadAnterior[personaje] = compatibilidadInicial;
                personaje.Compatibilidad.CompatibilidadValor = compatibilidadInicial;
            }
            MostrarPantallaInicial();
            Console.Clear();
            SeleccionarPersonaje();
        }
    
    private void MostrarPantallaInicial()
        {
            Console.WriteLine("La seleccion");

            Console.ReadKey();
            Console.Clear();
        }

 private void SeleccionarPersonaje()
        {
            Console.Clear();
            Console.WriteLine("Seleccione su personaje:");
            for (int i = 0; i < personajes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {personajes[i].Datos.Nombre} de {personajes[i].Datos.Provincia} (Edad: {personajes[i].Datos.Edad})");
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

        private double CalcularCompatibilidad(Personaje personaje)
        {
            double maxDiferencia = 10;
            double diferenciaInteligencia = Math.Abs(personaje.Caracteristicas.Inteligencia - principe.Caracteristicas.Inteligencia);
            double diferenciaAtractivo = Math.Abs(personaje.Caracteristicas.Atractivo - principe.Caracteristicas.Atractivo);
            double diferenciaCarisma = Math.Abs(personaje.Caracteristicas.Carisma - principe.Caracteristicas.Carisma);

            double compatibilidadInteligencia = (maxDiferencia - diferenciaInteligencia) / maxDiferencia;
            double compatibilidadAtractivo = (maxDiferencia - diferenciaAtractivo) / maxDiferencia;
            double compatibilidadCarisma = (maxDiferencia - diferenciaCarisma) / maxDiferencia;

            double compatibilidadTotal = (compatibilidadInteligencia + compatibilidadAtractivo + compatibilidadCarisma) / 4.0;

            if (personaje.Caracteristicas.Hobbie == principe.Caracteristicas.Hobbie)
            {
                compatibilidadTotal += 0.25;
            }

            return compatibilidadTotal * 100;
        }


}


}