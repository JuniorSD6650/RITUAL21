using System.Numerics;
using Raylib_cs;
using Ritual21.Models;

namespace Ritual21.Core;

// Clase para gestionar el movimiento visual de las cartas
public class CartaEnVuelo
{
    // Usamos '!' para decirle a C# que confíe en que daremos valor a estos campos
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

    // Lista de animaciones activas
    public List<CartaEnVuelo> AnimacionesActivas = new();

    public void Actualizar()
    {
        // 1. LÓGICA DE ANIMACIONES (Mover las cartas cuadro a cuadro)
        for (int i = AnimacionesActivas.Count - 1; i >= 0; i--)
        {
            var anim = AnimacionesActivas[i];

            // Suavizado de movimiento (Lerp)
            anim.PosicionActual = Vector2.Lerp(anim.PosicionActual, anim.Destino, 0.15f);

            // Crecimiento visual
            if (anim.EscalaActual < 0.25f) anim.EscalaActual += 0.01f;

            // Al llegar al destino
            if (Vector2.Distance(anim.PosicionActual, anim.Destino) < 1.0f)
            {
                anim.Propietario.Mano.Add(anim.Info);
                anim.Propietario.Puntos += anim.Info.Valor;
                AnimacionesActivas.RemoveAt(i);
            }
        }

        // 2. DESBLOQUEO DE INTERFAZ
        if (BloqueadoPorAnimacion && AnimacionesActivas.Count == 0 && Raylib.GetTime() >= TiempoEspera)
        {
            BloqueadoPorAnimacion = false;
        }

        // 3. CAPTURA DE TECLADO
        if (Estado == EstadoJuego.REGISTRO_CANTIDAD || Estado == EstadoJuego.REGISTRO_NOMBRES)
            ManejarTeclado();
    }

    // --- MÉTODOS DE LÓGICA QUE FALTABAN ---

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
            // Reparto inicial instantáneo para no saturar de animaciones al empezar
            for (int i = 0; i < 2; i++)
            {
                Carta c = MazoActual.RobarCarta();
                j.Mano.Add(c);
                j.Puntos += c.Valor;
            }
        }
        IndiceActual = 0;
        RealizoAccionEsteTurno = false;
        Estado = EstadoJuego.ESPERA_TURNO;
    }

    public void RobarConAnimacion(Jugador j)
    {
        Carta c = MazoActual.RobarCarta();
        var anim = new CartaEnVuelo
        {
            Info = c,
            Propietario = j,
            PosicionActual = new Vector2(400, 300), // Sale del mazo (centro)
            Destino = new Vector2(50 + (j.Mano.Count * 110), 140)
        };

        AnimacionesActivas.Add(anim);
        BloqueadoPorAnimacion = true;
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
            CartaInspeccionada.ActivarHabilidad(Participantes[IndiceActual], Participantes);
            CartaInspeccionada.HabilidadUsada = true;
            CartaInspeccionada = null;
        }
    }

    public void RobarPara(Jugador j)
    {
        Carta c = MazoActual.RobarCarta();

        j.Mano.Add(c);
        j.Puntos += c.Valor;

        RealizoAccionEsteTurno = true;
    }
}