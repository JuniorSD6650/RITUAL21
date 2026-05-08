using Ritual21.Models;
using Raylib_cs;

namespace Ritual21.Core;

public class GameController
{
    public EstadoJuego Estado = EstadoJuego.MENU_INICIAL;
    public List<Jugador> Participantes = new();
    public Mazo MazoActual = new();
    public int IndiceActual = 0;
    public string InputTexto = "";
    public int TotalJugadores = 0;

    public Carta? CartaInspeccionada = null;
    public double TiempoEspera = 0;
    public bool BloqueadoPorAnimacion = false;

    public bool RealizoAccionEsteTurno = false;

    public void Actualizar()
    {
        if (BloqueadoPorAnimacion && Raylib.GetTime() >= TiempoEspera)
        {
            BloqueadoPorAnimacion = false;
        }

        if (Estado == EstadoJuego.REGISTRO_CANTIDAD || Estado == EstadoJuego.REGISTRO_NOMBRES)
            ManejarTeclado();
    }

    private void ManejarTeclado()
    {
        int tecla = Raylib.GetCharPressed();
        while (tecla > 0)
        {
            if (tecla >= 32 && tecla <= 125) InputTexto += (char)tecla;
            tecla = Raylib.GetCharPressed();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && InputTexto.Length > 0)
            InputTexto = InputTexto[..^1];
    }

    public void InicializarMazo()
    {
        MazoActual = new Mazo();
        MazoActual.Barajar();
        Participantes.Clear();
    }

    public void ComenzarDuelo()
    {
        MazoActual.Barajar();
        foreach (var j in Participantes)
        {
            j.Mano.Clear();
            j.Puntos = 0;
            j.SePlanto = false;
            for (int i = 0; i < 2; i++) RobarPara(j);
        }
        RealizoAccionEsteTurno = false;
        Estado = EstadoJuego.ESPERA_TURNO;
    }

    public void RobarPara(Jugador j)
    {
        Carta c = MazoActual.RobarCarta();
        j.Mano.Add(c);
        j.Puntos += c.Valor;

        RealizoAccionEsteTurno = true;
    }

    public void PasarSiguiente()
    {
        RealizoAccionEsteTurno = false;

        int intentos = 0;
        do
        {
            IndiceActual = (IndiceActual + 1) % Participantes.Count;
            intentos++;
        } while ((Participantes[IndiceActual].SePlanto || Participantes[IndiceActual].Puntos >= 21) && intentos < Participantes.Count);

        if (Participantes.All(j => j.SePlanto || j.Puntos >= 21)) Estado = EstadoJuego.FINAL;
        else Estado = EstadoJuego.ESPERA_TURNO;
    }

    public void ActivarHabilidadInspeccionada()
    {
        if (CartaInspeccionada != null && !CartaInspeccionada.HabilidadUsada)
        {
            // Ejecutamos el polimorfismo que ya programaste
            CartaInspeccionada.ActivarHabilidad(Participantes[IndiceActual], Participantes);
            CartaInspeccionada.HabilidadUsada = true;
            CartaInspeccionada = null; // Cerramos la inspección tras usarla
        }
    }
}