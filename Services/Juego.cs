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
        private Princesa princesa;
        private PersonajesJson personajesJson;
        private HistorialJson historialJson;
        private List<string> historial;
        private Dictionary<Personaje, double> compatibilidadAnterior;
        private List<Personaje> personajesEliminados;

        public Juego()
        {
            fabrica = new FabricaDePersonajes();
            personajes = new List<Personaje>();
            personajesJson = new PersonajesJson();
            historialJson = new HistorialJson();    
            historial = new List<string>();
            compatibilidadAnterior = new Dictionary<Personaje, double>();
            personajesEliminados = new List<Personaje>();     
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
            
            princesa = await fabrica.CrearPrincesa();

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
                CentrarTexto($"Tu personaje: {compatibilidadPrincipalActual:F2}% (Cambio: {cambioCompatibilidadPrincipal:F2}%)", 10);
                compatibilidadAnterior[personajePrincipal] = compatibilidadPrincipalActual;
                CentrarTexto("Presiona cualquier tecla para continuar...", 13);
                Console.ReadKey();
                if (compatibilidadPrincipalActual >= 100.0)
                {
                    Console.Clear();
                    ganadorPrematuro = true;
                    Console.WriteLine($"¡La {princesa.Datos.Nombre} no podía quitar los ojos de ti y terminó el concurso antes de tiempo para casarse contigo! Ahora eres la princesa.");
                }
                else
                {
                    Console.Clear();
                    eliminado = EliminarPersonajesConMenorCompatibilidad(personajePrincipal);
                    semana++;
                    CentrarTexto("Presiona cualquier tecla para continuar...", 6);
                    Console.ReadKey();
                }
            }

            if (!eliminado && EsGanador(personajePrincipal) && !ganadorPrematuro)
            {
                Console.Clear();               
                string mensajeFinal=$"¡Felicidades! La {princesa.Datos.Nombre} te ha seleccionado como su nuevo esposo, ahora eres príncipe.";
                CentrarTexto(mensajeFinal);
            }
            else
            {
                Console.Clear();
                string mensajeFinal=$"Lo siento, no has sido seleccionado por la {princesa.Datos.Nombre}.";
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
            string[] opciones = { "Mostrar atributos de la princesa", "Reintentar", "Jugar nuevo juego", "Mostrar orden de personajes eliminados", "Cerrar Juego" };
            int seleccion = MostrarOpcionesYObtenerSeleccion(opciones, false);
            bool jugarDeNuevo=false;
            switch (seleccion)
            {
                case 0:
                    Console.Clear();
                    MostrarAtributosPrincesa();
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
                    LimpiarDatos();
                    break;
            }
            return jugarDeNuevo;
        }

        private void MostrarOrdenEliminados()
        {
            Console.Clear();
            CentrarTexto("Orden de personajes eliminados:",-7);
            int i=1;
            foreach (var personaje in personajesEliminados)
            {
                string registro = $"{personaje.Datos.Nombre} de {personaje.Datos.Provincia}";
                CentrarTexto(registro,i-7);
                i++;
            }


            CentrarTexto("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey();
            MostrarMenu();
        }

        private void MostrarCompatibilidadConPrincipe()
        {
            Console.Clear();
            string titulo = "Compatibilidad con la princesa:";
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
            double diferenciaInteligencia = Math.Abs(personaje.Caracteristicas.Inteligencia - princesa.Caracteristicas.Inteligencia);
            double diferenciaAtractivo = Math.Abs(personaje.Caracteristicas.Atractivo - princesa.Caracteristicas.Atractivo);
            double diferenciaCarisma = Math.Abs(personaje.Caracteristicas.Carisma - princesa.Caracteristicas.Carisma);

            double compatibilidadInteligencia = (maxDiferencia - diferenciaInteligencia) / maxDiferencia;
            double compatibilidadAtractivo = (maxDiferencia - diferenciaAtractivo) / maxDiferencia;
            double compatibilidadCarisma = (maxDiferencia - diferenciaCarisma) / maxDiferencia;

            double compatibilidadTotal = (compatibilidadInteligencia + compatibilidadAtractivo + compatibilidadCarisma) / 4.0;

            if (personaje.Caracteristicas.Hobbie == princesa.Caracteristicas.Hobbie)
            {
                compatibilidadTotal += 0.25;
            }

            return compatibilidadTotal * 100;
        }

        private void ElegirHobbie(Personaje personaje)
        {
            var random = new Random();
            Dictionary<Hobbie, List<string>> hobbiesOpciones = new Dictionary<Hobbie, List<string>>()
            {
                { Hobbie.Deporte, new List<string> { "Practicar fútbol", "Ir al gimnasio", "Hacer yoga" } },
                { Hobbie.Fotografia, new List<string> { "Tomar fotos en la naturaleza", "Asistir a un taller de fotografía", "Editar fotos en el ordenador" } },
                { Hobbie.Cine, new List<string> { "Ver una película", "Ir a un festival de cine", "Leer críticas de cine" } },
                { Hobbie.Ajedrez, new List<string> { "Jugar una partida de ajedrez", "Estudiar aperturas de ajedrez", "Participar en un torneo de ajedrez" } },
                { Hobbie.Videojuegos, new List<string> { "Jugar pvp de Minecraft", "Comprar un mando nuevo de consola", "Transmitir una partida en vivo" } }
            };

            List<string> acciones = new List<string>();
            foreach (var opciones in hobbiesOpciones.Values)
            {
                acciones.Add(opciones.OrderBy(x => random.Next()).First());
            }
            acciones = acciones.OrderBy(x => random.Next()).ToList();

            string[] botonesAcciones = acciones.Select(a => $"⋆˖⁺‧₊☽ {a} ☾₊‧⁺˖⋆").ToArray();
            Console.WriteLine("Seleccione una acción:");
            int seleccionAccion = MostrarOpcionesYObtenerSeleccion(botonesAcciones, true);

            string accionSeleccionada = acciones[seleccionAccion];
            Hobbie hobbieSeleccionado = hobbiesOpciones.FirstOrDefault(x => x.Value.Contains(accionSeleccionada)).Key;
            personaje.Caracteristicas.Hobbie = hobbieSeleccionado;

            historial.Add($"Acción: {personaje.Datos.Nombre} realizó {accionSeleccionada}.");
        }

        private void RealizarAccionesSemanales()
        {
            int accionesRealizadas = 0;
            var random = new Random();
            Dictionary<string, List<string>> accionesOpciones = new Dictionary<string, List<string>>()
            {
                { "Subir inteligencia", new List<string> { "Leer libro de fisica", "Estudiar matemáticas","Asistir a un congreso" } },
                { "Bajar inteligencia", new List<string> { "Ver TV basura", "Dejar de estudiar", "Proclamar que sos el más inteligente" } },
                { "Subir atractivo", new List<string> { "Hacer ejercicio", "Ir a un spa", "Arreglarse las uñas" } },
                { "Bajar atractivo", new List<string> { "No asearse", "Ensuciarse con comida", "Tener celos de los demás"} },
                { "Subir carisma", new List<string> { "Tomar clases de actuación", "Leer sobre liderazgo", "Mantenerse positivo" } },
                { "Bajar carisma", new List<string> { "Ser grosero", "Ignorar a la gente", "Ser sarcástico en exceso" } },
                { "Elegir hobbie de la semana", new List<string> { "Elegir qué se practicará en el día" } }
            };

            List<string> opciones = accionesOpciones.Keys.ToList();
            opciones = opciones.OrderBy(x => random.Next()).ToList();
            while (accionesRealizadas < 3)
            {
                Console.Clear();
                MostrarBarraSuperior(personajePrincipal);
                Console.WriteLine("\nSeleccione una acción para realizar:");
                List<string> botones = opciones.Select(o => $"⋆˖⁺‧₊☽ {accionesOpciones[o].OrderBy(a => random.Next()).First()} ☾₊‧⁺˖⋆").ToList();
                int seleccion = MostrarOpcionesYObtenerSeleccion(botones.ToArray(), true);
                string opcionSeleccionada = opciones[seleccion];

                int accion = -1;
                switch (opcionSeleccionada)
                {
                    case "Subir inteligencia":
                        accion = 1;
                        break;
                    case "Bajar inteligencia":
                        accion = 2;
                        break;
                    case "Subir atractivo":
                        accion = 3;
                        break;
                    case "Bajar atractivo":
                        accion = 4;
                        break;
                    case "Subir carisma":
                        accion = 5;
                        break;
                    case "Bajar carisma":
                        accion = 6;
                        break;
                    case "Elegir hobbie de la semana":
                        accion = 7;
                        break;
                }

                bool accionValida = GeneradorDeAtributos.RealizarAccion(personajePrincipal, accion, true);
                if (accionValida)
                {
                    string descripcionAccion;
                    if (accion == 7)
                    {
                        ElegirHobbie(personajePrincipal);
                        descripcionAccion = $"eligió un nuevo hobbie: {personajePrincipal.Caracteristicas.Hobbie}.";
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
            int i=0;
            foreach (var personaje in personajesAEliminar)
            {
                if (personaje == personajePrincipal)
                {
                    eliminado = true;
                    string mensaje = $"Has sido eliminado. Compatibilidad: {CalcularCompatibilidad(personajePrincipal):F2}%";
                    CentrarTexto(mensaje,i);
                    personajesEliminados.Add(personajePrincipal);
                    i++;
                }
                else
                {
                    personajes.Remove(personaje);
                    string mensaje = $"Eliminado: {personaje.Datos.Nombre} de {personaje.Datos.Provincia}";
                    CentrarTexto(mensaje,i);
                    personajesEliminados.Add(personaje);
                    i++;
                }
                historial.Add($"Eliminado: {personaje.Datos.Nombre} de {personaje.Datos.Provincia}.");
            }

            return eliminado;
        }

        private bool EsGanador(Personaje personaje)
        {
            var diferenciaPrincipal = Math.Abs(personaje.Caracteristicas.Inteligencia - princesa.Caracteristicas.Inteligencia) +
                                      Math.Abs(personaje.Caracteristicas.Atractivo - princesa.Caracteristicas.Atractivo) +
                                      Math.Abs(personaje.Caracteristicas.Carisma - princesa.Caracteristicas.Carisma);

            var diferenciasRestantes = personajes.Select(p => Math.Abs(p.Caracteristicas.Inteligencia - princesa.Caracteristicas.Inteligencia) +
                                                              Math.Abs(p.Caracteristicas.Atractivo - princesa.Caracteristicas.Atractivo) +
                                                              Math.Abs(p.Caracteristicas.Carisma - princesa.Caracteristicas.Carisma)).ToList();

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

        private void MostrarAtributosPrincesa()
        {
            CentrarTexto(
            @$"     Atributos de la {princesa.Datos.Nombre}:
            Inteligencia: {princesa.Caracteristicas.Inteligencia}
            Atractivo: {princesa.Caracteristicas.Atractivo}
            Carisma: {princesa.Caracteristicas.Carisma}
            Hobbie: {princesa.Caracteristicas.Hobbie}
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