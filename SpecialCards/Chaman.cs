using Ritual21.Models;

namespace Ritual21.SpecialCards;

public class Chaman : Carta
{
    public Chaman() : base("Chamán Ancestral", 1) { }
    public override bool TieneHabilidad => true;
    public override void ActivarHabilidad(Jugador actual, List<Jugador> todos)
    {
        if (actual.Puntos > 5)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("😇 ¡MILAGRO! El Chamán ha detectado una sobrecarga de energía.");
            Console.WriteLine($"{actual.Nombre} es salvado de la expulsión.");
            actual.Puntos -= 5;
            if (actual.Puntos < 0) actual.Puntos = 0;
            Console.ResetColor();
        } 
    }
}
