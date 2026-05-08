namespace Ritual21.Models;

public class Jugador
{
    public string Nombre { get; private set; }
    public int Puntos { get; set; } = 0;
    public bool SePlanto { get; set; } = false;

    public List<Carta> Mano { get; set; } = new List<Carta>();

    public Jugador(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("¡Un invocador debe tener nombre!");
        Nombre = nombre;
    }
}