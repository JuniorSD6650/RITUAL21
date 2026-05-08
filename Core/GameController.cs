using System.Numerics;
using Raylib_cs;
using Ritual21.Models;

namespace Ritual21.Core;

public class CartaEnVuelo 
{
    public Carta Info { get; set; } = null!;
    public Jugador Propietario { get; set; } = null!; 
    public Vector2 PosicionActual { get; set; }
    public Vector2 Destino { get; set; }
    public float EscalaActual { get; set; } = 0.05f;
}

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

    public List<CartaEnVuelo> AnimacionesActivas = new();

    public void Actualizar()
    {
        // 1. LÓGICA DE ANIMACIONES
        for (int i = AnimacionesActivas.Count - 1; i >= 0; i--)
        {
            var anim = AnimacionesActivas[i];
            anim.PosicionActual = Vector2.Lerp(anim.PosicionActual, anim.Destino, 0.15f);
            if (anim.EscalaActual < 0.25f) anim.EscalaActual += 0.01f;

            if (Vector2.Distance(anim.PosicionActual, anim.Destino) < 1.0f)
            {
                anim.Propietario.Mano.Add(anim.Info);
                anim.Propietario.Puntos += anim.Info.Valor;
                AnimacionesActivas.RemoveAt(i);
            }
        }

        // 2. DESBLOQUEO
        if (BloqueadoPorAnimacion && AnimacionesActivas.Count == 0 && Raylib.GetTime() >= TiempoEspera)
            BloqueadoPorAnimacion = false;

        // 3. ENTRADA DE TEXTO
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
        InputTexto = "";
    }

    public void ComenzarDuelo()
    {
        MazoActual.Barajar();
        foreach (var j in Participantes)
        {
            j.Mano.Clear(); j.Puntos = 0; j.SePlanto = false;
            for (int i = 0; i < 2; i++) RobarPara(j);
        }
        IndiceActual = 0;
        RealizoAccionEsteTurno = false;
        Estado = EstadoJuego.ESPERA_TURNO;
    }

    public void RobarPara(Jugador j)
    {
        Carta c = MazoActual.RobarCarta();
        j.Mano.Add(c);
        j.Puntos += c.Valor;
    }

    public void RobarConAnimacion(Jugador j)
    {
        Carta c = MazoActual.RobarCarta();
        AnimacionesActivas.Add(new CartaEnVuelo {
            Info = c, Propietario = j,
            PosicionActual = new Vector2(400, 300),
            Destino = new Vector2(50 + (j.Mano.Count * 110), 140)
        });
        BloqueadoPorAnimacion = true;
        RealizoAccionEsteTurno = true;
    }

    public void PasarSiguiente()
    {
        RealizoAccionEsteTurno = false;
        int intentos = 0;
        do {
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
            CartaInspeccionada.ActivarHabilidad(Participantes[IndiceActual], Participantes);
            CartaInspeccionada.HabilidadUsada = true;
            CartaInspeccionada = null; 
        }
    }
}