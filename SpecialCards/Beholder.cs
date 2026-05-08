using Ritual21.Models;

namespace Ritual21.SpecialCards;

public class Beholder : Carta
{
    // Iniciamos con valor 0 porque se calculará al ser invocado
    public Beholder() : base("👁️ THE BEHOLDER (JEFE)", 0) { }
    public override bool TieneHabilidad => true;
    public override void ActivarHabilidad(Jugador actual, List<Jugador> todos)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("👁️ ¡MIRADA DE ESPEJO! The Beholder emerge del vacío...");

        // Lógica: Busca el puntaje más alto entre todos los jugadores para copiarlo
        int puntajeMaximo = 0;
        foreach (var j in todos)
        {
            if (j.Puntos > puntajeMaximo && j.Puntos <= 21)
                puntajeMaximo = j.Puntos;
        }

        // El Beholder copia ese valor para ayudar (o hundir) al invocador
        this.Valor = puntajeMaximo > 0 ? puntajeMaximo / 2 : 5;
        actual.Puntos += this.Valor;

        Console.WriteLine($"🔍 The Beholder ha copiado el flujo de energía. Valor otorgado: {this.Valor}");
        Console.ResetColor();
    }
}