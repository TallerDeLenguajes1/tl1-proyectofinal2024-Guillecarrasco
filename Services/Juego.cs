using ProyectoJuegoDeRol.Models;
using ProyectoJuegoDeRol.Services.Persistencia;
using ProyectoJuegoDeRol.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            bool jugarDeNuevo = false;
            do
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
            MostrarVideoIntro();
            Console.Clear();
            jugarDeNuevo= SeleccionarPersonaje();
            }while(jugarDeNuevo );
        }
    
        private void MostrarPantallaInicial()
        {
            string title = @"
                █████ █                                       ███                                                                
            ██████  █                                         ███                                   █          █                 
            ██   █  █                                           ██                                  ███        ██                  
            █    █  █                                            ██                                   █        ██                   
                █  █                                             ██                                                               
            ██ ██              ████          ████      ███    ██      ███       ████      ████   ███       ████   ███  ████    
            ██ ██             █ ███  █      █ ████ █  █ ███   ██     █ ███     █ ███  █  █ ███  █ ███     █ ███  █ ████ ████ █ 
            ██ ██            █   ████      ██  ████  █   ███  ██    █   ███   █   ████  █   ████   ██    █   ████   ██   ████  
            ██ ██           ██    ██      ████      ██    ███ ██   ██    ███ ██        ██          ██   ██    ██    ██    ██   
            ██ ██           ██    ██        ███     ████████  ██   ████████  ██        ██          ██   ██    ██    ██    ██   
            █  ██           ██    ██          ███   ███████   ██   ███████   ██        ██          ██   ██    ██    ██    ██   
            █  █            ██    ██            ███ ██        ██   ██        ██        ██          ██   ██    ██    ██    ██   
            ████           █ ██    ██       ████  ██ ████    █ ██   ████    █ ███     █ ███     █   ██   ██    ██    ██    ██   
            █  █████████████   █████ ██     █ ████ █   ███████  ███ █ ███████   ███████   ███████    ███ █ ██████     ███   ███  
            █     █████████      ███   ██       ████     █████    ███   █████     █████     █████      ███   ████       ███   ███ 
            █                                                                                                                     
            ██                                                                                                                   
            ";
            Console.Clear();
            CentrarTexto(title);
            CentrarTexto("Toque cualquier tecla para empezar", 8);
            Console.SetCursorPosition((Console.WindowWidth / 2) - 1, Console.CursorTop + 2);
            Console.ReadKey();
        }
        
        private void CentrarTexto(string texto, int centroHaciaAbajo = 0)
        {
            string[] lineas = texto.Split('\n');
            int top = (Console.WindowHeight - lineas.Length) / 2 + centroHaciaAbajo;
            foreach (var linea in lineas)
            {
                int left = (Console.WindowWidth - linea.Length) / 2;
                Console.SetCursorPosition(left, top++);
                Console.WriteLine(linea);
            }
        }
        private void MostrarVideoIntro()
        {
            Console.WriteLine("Mostrando video de introducción...");
            System.Threading.Thread.Sleep(3000);
        }

        private bool SeleccionarPersonaje()
        {
            Console.Clear();
            string[] opcionesPersonajes = personajes.Select(p => $"{p.Datos.Nombre} de {p.Datos.Provincia} (Edad: {p.Datos.Edad})").ToArray();
            Console.WriteLine("Seleccione el personaje que quiera ser:");
            int seleccion = MostrarOpcionesYObtenerSeleccion(opcionesPersonajes, false);
            personajePrincipal = personajes[seleccion];
            personajes.RemoveAt(seleccion);
            Console.Clear();
            bool jugarDeNuevo = Jugar();
            return jugarDeNuevo;

        }            


        private bool Jugar()
        {
            int semana = 1;
            bool eliminado = false;
            bool ganadorPrematuro = false;

            while (personajes.Count > 3 && !eliminado && !ganadorPrematuro)
            {
                Console.Clear();
                Console.WriteLine($"\nSemana {semana}:");

                RealizarAccionesSemanales();

                foreach (var personaje in personajes)
                {
                    RealizarAccionesAleatorias(personaje);
                }

                MostrarCompatibilidadConPrincipe();       

                double compatibilidadPrincipalActual = CalcularCompatibilidad(personajePrincipal);
                double cambioCompatibilidadPrincipal = compatibilidadPrincipalActual - compatibilidadAnterior[personajePrincipal];
                Console.WriteLine($"Tu personaje: {compatibilidadPrincipalActual:F2}% (Cambio: {cambioCompatibilidadPrincipal:F2}%)");
                compatibilidadAnterior[personajePrincipal] = compatibilidadPrincipalActual;

                if (compatibilidadPrincipalActual >= 100.0)
                {
                    Console.Clear();
                    ganadorPrematuro = true;
                    Console.WriteLine("¡El príncipe no podía quitar los ojos de ti y terminó el concurso antes de tiempo para casarse contigo! Ahora eres la princesa.");
                }
                else
                {

                    eliminado = EliminarPersonajesConMenorCompatibilidad(personajePrincipal);
                    semana++;
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }

            if (!eliminado && EsGanador(personajePrincipal) && !ganadorPrematuro)
            {
                Console.Clear();               
                string mensajeFinal="¡Felicidades! Has sido seleccionada como la princesa.";
                CentrarTexto(mensajeFinal);
            }
            else if(eliminado && !ganadorPrematuro)
            {
                Console.Clear();
                string mensajeFinal="Lo siento, no has sido seleccionada como la princesa.";
                CentrarTexto(mensajeFinal);
            }
            CentrarTexto("Presiona cualquier tecla para continuar...",3);
            Console.ReadKey();
            Console.Clear();
            bool jugarDeNuevo = MostrarMenu();
            return jugarDeNuevo;           
        }

        private bool MostrarMenu()
        {
            string[] opciones = { "Mostrar atributos del príncipe", "Reintentar", "Jugar nuevo juego", "Mostrar orden de personajes eliminados", "Cerrar Juego" };
            int seleccion = MostrarOpcionesYObtenerSeleccion(opciones, false);
            bool jugarDeNuevo=false;
            switch (seleccion)
            {
                case 0:
                    Console.Clear();
                    MostrarAtributosPrincipe();
                    break;
                case 1:
                    jugarDeNuevo = true;
                    Console.Clear(); 
                    break;
                case 2:
                    jugarDeNuevo = true; 
                    Console.Clear();
                    LimpiarDatos();
                    break;
                case 3:
                    Console.Clear();
                    MostrarOrdenEliminados();
                    break;
                case 4:
                    Console.Clear();
                    break;
            }
            return jugarDeNuevo;
        }

        private void MostrarOrdenEliminados()
        {
            Console.Clear();
            CentrarTexto("Orden de personajes eliminados:");

            foreach (var registro in historial)
            {
                CentrarTexto(registro);
            }

            CentrarTexto("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey();
            MostrarMenu();
        }

        private void MostrarCompatibilidadConPrincipe()
        {
            Console.Clear();
            string titulo = "Compatibilidad con el príncipe:";
            int consolaAncho = Console.WindowWidth;
            int paddingTitulo = (consolaAncho - titulo.Length) / 2;
            Console.WriteLine(new string(' ', paddingTitulo) + titulo);
            Console.WriteLine();
            foreach (var personaje in personajes)
            {
                double compatibilidadActual = CalcularCompatibilidad(personaje);
                double cambioCompatibilidad = compatibilidadActual - compatibilidadAnterior[personaje];
                string textoCompatibilidad = $"{personaje.Datos.Nombre}: {compatibilidadActual:F2}% (Cambio: {cambioCompatibilidad:F2}%)";
                int paddingCompatibilidad = (consolaAncho - textoCompatibilidad.Length) / 2;
                Console.WriteLine(new string(' ', paddingCompatibilidad) + textoCompatibilidad);
                compatibilidadAnterior[personaje] = compatibilidadActual;
            }
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
            string[] opcionesHobbies = hobbies.Select(h => h.ToString()).ToArray();
            string[] botonesHobbies = opcionesHobbies.Select(h => $"⋆˖⁺‧₊☽ {h} ☾₊‧⁺˖⋆").ToArray();
            Console.WriteLine("Seleccione un hobbie:");
            int seleccion = MostrarOpcionesYObtenerSeleccion(botonesHobbies, true);
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

                int seleccion = MostrarOpcionesYObtenerSeleccion(botones, true);

                int accion = seleccion + 1;

                bool accionValida = GeneradorDeAtributos.RealizarAccion(personajePrincipal, accion, true);
                if (accionValida)
                {
                    string descripcionAccion;
                    if (accion == 7)
                    {
                        descripcionAccion = $"eligió un nuevo hobbie: {personajePrincipal.Caracteristicas.Hobbie}.";
                        ElegirHobbie(personajePrincipal);
                    }
                    else
                    {
                        descripcionAccion = $"realizó acción {accion}.";
                    }
                    historial.Add($"Acción {accionesRealizadas + 1}: {personajePrincipal.Datos.Nombre} {descripcionAccion}");
                    accionesRealizadas++;
                }
            }
        }

        private int MostrarOpcionesYObtenerSeleccion(string[] opciones, bool Barra)
        {
            int seleccion = 0;
            while (true)
            {
                Console.Clear();
                if(Barra){
                    MostrarBarraSuperior(personajePrincipal);
                }
                

                int anchoConsola = Console.WindowWidth;
                int alturaConsola = Console.WindowHeight;
                int alturaBotones = opciones.Length * 2;
                int paddingVertical = (alturaConsola - (alturaBotones + 1)) / 2;

                for (int i = 0; i < paddingVertical; i++)
                {
                    Console.WriteLine();
                }
                
                string marco1 = "╔════════════ ❀•°❀°•❀ ════════════╗";
                int padding2 = (anchoConsola - marco1.Length) / 2;
                string titulo = "Seleccione una opcion:";
                int padding25 = (anchoConsola - titulo.Length) / 2;
                Console.WriteLine(new string(' ', padding25) + titulo);
                Console.WriteLine(new string(' ', padding2) + marco1);
                
                for (int i = 0; i < opciones.Length; i++)
                {
                    string opcion = opciones[i];
                    int padding = (anchoConsola - opcion.Length) / 2;

                    if (i == seleccion)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(new string(' ', padding) + opcion);

                    Console.ResetColor();
                }

                string marco2 = "╚════════════ ❀•°❀°•❀ ════════════╝";
                int padding3 = (anchoConsola - marco2.Length) / 2;
                Console.WriteLine(new string(' ', padding3) + marco2);

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    seleccion = (seleccion == 0) ? opciones.Length - 1 : seleccion - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    seleccion = (seleccion == opciones.Length - 1) ? 0 : seleccion + 1;
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
            CentrarTexto(@$"Atributos de {personaje.Datos.Nombre}
            Inteligencia: {personaje.Caracteristicas.Inteligencia}
            Atractivo: {personaje.Caracteristicas.Atractivo}
            Carisma: {personaje.Caracteristicas.Carisma}
            Hobbie: {personaje.Caracteristicas.Hobbie}");
        }

        private void MostrarAtributosPrincipe()
        {
            CentrarTexto(@$"Atributos del Príncipe:
            Inteligencia: {principe.Caracteristicas.Inteligencia}
            Atractivo: {principe.Caracteristicas.Atractivo}
            Carisma: {principe.Caracteristicas.Carisma}
            Hobbie: {principe.Caracteristicas.Hobbie}
            Presiona cualquier tecla para volver al menú...
            ");
            Console.ReadKey();
            MostrarMenu();
        }

        private void LimpiarDatos()
        {
            personajesJson.BorrarDatos("Data/personajes.json");
            historialJson.BorrarDatos("Data/historial.json");
        }
    }
}