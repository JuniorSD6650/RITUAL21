using Ritual21.Models;

namespace Ritual21.SpecialCards;

public class Chaman : Carta
{
    public Chaman() : base("Chamán Ancestral", 11) { }
    public override bool TieneHabilidad => true;
    public override void ActivarHabilidad(Jugador actual, List<Jugador> todos)
    {

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("😇 ¡MILAGRO! El Chamán ha detectado una sobrecarga de energía.");
        Console.WriteLine($"{actual.Nombre} es salvado de la expulsión.");
        actual.Puntos -= 5; // Restamos puntos para mantenerlo en el juego 
        Console.ResetColor();

    }
}