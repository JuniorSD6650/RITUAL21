namespace Ritual21.Models;

public class Carta
{
    public string Nombre { get; set; }
    public int Valor { get; set; }
    public bool HabilidadUsada { get; set; } = false;
    public virtual bool TieneHabilidad => false;
    public Carta(string nombre, int valor)
    {
        Nombre = nombre;
        Valor = valor;
    }

    // Este método lo cambiaremos en los Demorfitos Legendarios
    public virtual void ActivarHabilidad(Jugador actual, List<Jugador> todos)
    {
        // Por defecto, las cartas comunes no hacen nada extra
    }
}