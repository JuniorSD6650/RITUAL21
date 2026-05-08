using Ritual21.Models;

namespace Ritual21.SpecialCards;

public class Espectro : Carta
{
    public Espectro() : base("Espectro Legendario", 1) { }
    public override bool TieneHabilidad => true;
    public override void ActivarHabilidad(Jugador actual, List<Jugador> todos)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"👻 ¡GRITO DE PÁNICO! {actual.Nombre} ha invocado al Espectro.");

        // Lógica: Buscar al jugador anterior para penalizarlo
        int miIndice = todos.IndexOf(actual);
        int indiceRival = miIndice == 0 ? todos.Count - 1 : miIndice - 1;

        Jugador rival = todos[indiceRival];

        Console.WriteLine($"👿 El rival {rival.Nombre} pierde sus últimos puntos acumulados.");
        rival.Puntos -= 5;
        if (rival.Puntos < 0) rival.Puntos = 0;

        Console.ResetColor();
    }
}