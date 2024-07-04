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
            historial = new List<string>();
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
            int semana = 1;
            bool eliminado = false;

            while (personajes.Count > 3 && !eliminado)
            {
                Console.Clear();
                Console.WriteLine($"\nSemana {semana}:");

                RealizarAccionesSemanales();

                foreach (var personaje in personajes)
                {
                    RealizarAccionesAleatorias(personaje);
                }

                Console.Clear();
                Console.WriteLine("\nCompatibilidad con el príncipe:");
                foreach (var personaje in personajes)
                {
                    double compatibilidadActual = CalcularCompatibilidad(personaje);
                    double cambioCompatibilidad = compatibilidadActual - compatibilidadAnterior[personaje];
                    Console.WriteLine($"{personaje.Datos.Nombre}: {compatibilidadActual:F2}% (Cambio: {cambioCompatibilidad:F2}%)");
                    compatibilidadAnterior[personaje] = compatibilidadActual;
                }

                double compatibilidadPrincipalActual = CalcularCompatibilidad(personajePrincipal);
                double cambioCompatibilidadPrincipal = compatibilidadPrincipalActual - compatibilidadAnterior[personajePrincipal];
                Console.WriteLine($"Tu personaje: {compatibilidadPrincipalActual:F2}% (Cambio: {cambioCompatibilidadPrincipal:F2}%)");
                compatibilidadAnterior[personajePrincipal] = compatibilidadPrincipalActual;

                eliminado = EliminarPersonajesConMenorCompatibilidad(personajePrincipal);

                historial.Add($"Semana {semana}: {personajePrincipal.Datos.Nombre} realizó acciones.");
                historialJson.GuardarHistorial(historial, "Data/historial.json");

                semana++;
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();
            }

            if (!eliminado && EsGanador(personajePrincipal))
            {
                Console.WriteLine("¡Felicidades! Has sido seleccionada como la princesa.");
            }
            else
            {
                Console.WriteLine("Lo siento, no has sido seleccionada como la princesa.");
            }

            MostrarAtributosPrincipe();
            LimpiarDatos();
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

        private void ElegirHobbie(Personaje personaje)
        {
            var hobbies = Enum.GetValues(typeof(Hobbie)).Cast<Hobbie>().ToList();
            Console.WriteLine("Seleccione un hobbie:");
            for (int i = 0; i < hobbies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {hobbies[i]}");
            }

            int seleccion;
            do
            {
                Console.WriteLine("Ingrese el número del hobbie que desea seleccionar:");
                seleccion = int.Parse(Console.ReadLine()) - 1;
            } while (seleccion < 0 || seleccion >= hobbies.Count);

            personaje.Caracteristicas.Hobbie = hobbies[seleccion];
        }

        private void RealizarAccionesSemanales()
        {
            int accionesRealizadas = 0;
            string[] opciones = {
                "Subir inteligencia", "Bajar inteligencia",
                "Subir atractivo", "Bajar atractivo", "Subir carisma",
                "Bajar carisma", "Elegir hobbie de la semana"
            };

            string[] botones = opciones.Select(o => $"⋆˖⁺‧₊☽ {o} ☾₊‧⁺˖⋆").ToArray();

            while (accionesRealizadas < 3)
            {
                Console.Clear();
                MostrarBarraSuperior(personajePrincipal);
                Console.WriteLine("\nSeleccione una acción para realizar:");

                int seleccion = MostrarOpcionesYObtenerSeleccion(botones);

                int accion = seleccion + 1;

                if (accion == 7)
                {
                    ElegirHobbie(personajePrincipal);
                    historial.Add($"Acción {accionesRealizadas + 1}: {personajePrincipal.Datos.Nombre} eligió un nuevo hobbie.");
                }
                else
                {
                    bool accionValida = GeneradorDeAtributos.RealizarAccion(personajePrincipal, accion, true);
                    if (accionValida)
                    {
                        historial.Add($"Acción {accionesRealizadas + 1}: {personajePrincipal.Datos.Nombre} realizó acción {accion}.");
                    }
                }

                accionesRealizadas++;
            }
        }

        private int MostrarOpcionesYObtenerSeleccion(string[] botones)
        {
            int seleccion = 0;
            while (true)
            {
                Console.Clear();
                MostrarBarraSuperior(personajePrincipal);
                Console.WriteLine("\nSeleccione una acción para realizar:");

                int anchoConsola = Console.WindowWidth;
                int alturaConsola = Console.WindowHeight;
                int alturaBotones = botones.Length * 2; 
                int paddingVertical = (alturaConsola - (alturaBotones + 1)) / 2;

                for (int i = 0; i < paddingVertical; i++)
                {
                    Console.WriteLine();
                }

                string marco1 = "╔══════ ❀•°❀°•❀ ══════╗";
                int padding2 = (anchoConsola - marco1.Length) / 2;
                Console.WriteLine(new string(' ', padding2) + marco1);

                for (int i = 0; i < botones.Length; i++)
                {
                    string boton = botones[i];
                    int padding = (anchoConsola - boton.Length) / 2;

                    if (i == seleccion)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(new string(' ', padding) + boton);

                    Console.ResetColor();
                    Console.WriteLine();
                }

                string marco2 = "╚══════ ❀•°❀°•❀ ══════╝";
                int padding3 = (anchoConsola - marco2.Length) / 2;
                Console.WriteLine(new string(' ', padding3) + marco2);

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    seleccion = (seleccion == 0) ? botones.Length - 1 : seleccion - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    seleccion = (seleccion == botones.Length - 1) ? 0 : seleccion + 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return seleccion;
        }


        private void MostrarBarraSuperior(Personaje personaje)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Personaje: {personaje.Datos.Nombre} | Inteligencia: {personaje.Caracteristicas.Inteligencia} | Atractivo: {personaje.Caracteristicas.Atractivo} | Carisma: {personaje.Caracteristicas.Carisma} | Hobbie: {personaje.Caracteristicas.Hobbie}");
            Console.SetCursorPosition(0, 1);
        }
        private void RealizarAccionesAleatorias(Personaje personaje)
        {
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                int accion = random.Next(1, 8);
                bool accionValida = GeneradorDeAtributos.RealizarAccion(personaje, accion, false);
                if (accionValida)
                {
                    historial.Add($"Acción {i + 1}: {personaje.Datos.Nombre} realizó acción {accion}.");
                }
                else
                {
                    i--;
                }
            }
        }
        private bool EliminarPersonajesConMenorCompatibilidad(Personaje personajePrincipal)
        {
            var compatibilidades = personajes.Select(p => new
            {
                Personaje = p,
                Compatibilidad = CalcularCompatibilidad(p)
            }).ToList();

            compatibilidades.Add(new { Personaje = personajePrincipal, Compatibilidad = CalcularCompatibilidad(personajePrincipal) });

            var personajesAEliminar = compatibilidades.OrderBy(c => c.Compatibilidad).Take(3).Select(c => c.Personaje).ToList();

            bool eliminado = false;
            foreach (var personaje in personajesAEliminar)
            {
                if (personaje == personajePrincipal)
                {
                    eliminado = true;
                    Console.WriteLine($"Has sido eliminada. Compatibilidad: {CalcularCompatibilidad(personajePrincipal):F2}%");
                }
                else
                {
                    personajes.Remove(personaje);
                    Console.WriteLine($"Eliminada: {personaje.Datos.Nombre} de {personaje.Datos.Provincia}");
                }
                historial.Add($"Eliminada: {personaje.Datos.Nombre} de {personaje.Datos.Provincia}.");
            }

            return eliminado;
        }

        private bool EsGanador(Personaje personaje)
        {
            var diferenciaPrincipal = Math.Abs(personaje.Caracteristicas.Inteligencia - principe.Caracteristicas.Inteligencia) +
                                      Math.Abs(personaje.Caracteristicas.Atractivo - principe.Caracteristicas.Atractivo) +
                                      Math.Abs(personaje.Caracteristicas.Carisma - principe.Caracteristicas.Carisma);

            var diferenciasRestantes = personajes.Select(p => Math.Abs(p.Caracteristicas.Inteligencia - principe.Caracteristicas.Inteligencia) +
                                                              Math.Abs(p.Caracteristicas.Atractivo - principe.Caracteristicas.Atractivo) +
                                                              Math.Abs(p.Caracteristicas.Carisma - principe.Caracteristicas.Carisma)).ToList();

            return diferenciaPrincipal <= diferenciasRestantes.Min();
        }
        private void MostrarAtributosPersonaje(Personaje personaje)
        {
            Console.WriteLine($"Atributos de {personaje.Datos.Nombre}:");
            Console.WriteLine($"Inteligencia: {personaje.Caracteristicas.Inteligencia}");
            Console.WriteLine($"Atractivo: {personaje.Caracteristicas.Atractivo}");
            Console.WriteLine($"Carisma: {personaje.Caracteristicas.Carisma}");
            Console.WriteLine($"Hobbie: {personaje.Caracteristicas.Hobbie}");
        }

        private void MostrarAtributosPrincipe()
        {
            Console.WriteLine($"Atributos del Príncipe:");
            Console.WriteLine($"Inteligencia: {principe.Caracteristicas.Inteligencia}");
            Console.WriteLine($"Atractivo: {principe.Caracteristicas.Atractivo}");
            Console.WriteLine($"Carisma: {principe.Caracteristicas.Carisma}");
            Console.WriteLine($"Hobbie: {principe.Caracteristicas.Hobbie}");
        }

        private void LimpiarDatos()
        {
            personajesJson.BorrarDatos("Data/personajes.json");
            historialJson.BorrarDatos("Data/historial.json");
        }
    }
}