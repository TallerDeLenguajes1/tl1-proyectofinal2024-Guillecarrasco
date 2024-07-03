using ProyectoJuegoDeRol.Models;



namespace ProyectoJuegoDeRol.Utils
{
public static class GeneradorDeAtributos
{
public static bool RealizarAccion(Personaje personaje, int accion, bool mostrarMensajes)
{
    bool accionValida = true;

    switch (accion)
    {
        case 1:
            if (personaje.Caracteristicas.Inteligencia < 10)
                personaje.Caracteristicas.Inteligencia++;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes subir más porque ya tienes la inteligencia al máximo.");
                accionValida = false;
            }
            break;
        case 2:
            if (personaje.Caracteristicas.Inteligencia > 0)
                personaje.Caracteristicas.Inteligencia--;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes bajar más porque ya tienes la inteligencia al mínimo.");
                accionValida = false;
            }
            break;
        case 3:
            if (personaje.Caracteristicas.Atractivo < 10)
                personaje.Caracteristicas.Atractivo++;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes subir más porque ya tienes el atractivo al máximo.");
                accionValida = false;
            }
            break;
        case 4:
            if (personaje.Caracteristicas.Atractivo > 0)
                personaje.Caracteristicas.Atractivo--;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes bajar más porque ya tienes el atractivo al mínimo.");
                accionValida = false;
            }
            break;
        case 5:
            if (personaje.Caracteristicas.Carisma < 10)
                personaje.Caracteristicas.Carisma++;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes subir más porque ya tienes el carisma al máximo.");
                accionValida = false;
            }
            break;
        case 6:
            if (personaje.Caracteristicas.Carisma > 0)
                personaje.Caracteristicas.Carisma--;
            else
            {
                if (mostrarMensajes)
                    Console.WriteLine("No puedes bajar más porque ya tienes el carisma al mínimo.");
                accionValida = false;
            }
            break;
        case 7:
                var hobbies = Enum.GetValues(typeof(Hobbie));
                personaje.Caracteristicas.Hobbie = (Hobbie)hobbies.GetValue(new Random().Next(hobbies.Length));
            break;
        default:
            if (mostrarMensajes)
                Console.WriteLine("Acción no válida");
            accionValida = false;
            break;
    }

    if (accionValida)
    {
        if (mostrarMensajes)
            Console.WriteLine("Acción realizada con éxito.");
        return true;
    }
    else
    {
        if (mostrarMensajes)
            Console.WriteLine("Inténtalo de nuevo.");
        return false;
    }
}
}
}


