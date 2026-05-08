using Raylib_cs;
using Ritual21.Core;
using Ritual21.UI;
using Ritual21.Models;
using System.Numerics;

GameController game = new GameController();

Raylib.InitWindow(800, 600, "Ritual 21: Deck Chronicles");
Raylib.SetTargetFPS(60);
VisualResourceManager.CargarRecursos("MonstruosAncestrales");

while (!Raylib.WindowShouldClose())
{
    // --- 1. LÓGICA Y ACTUALIZACIÓN ---
    game.Actualizar();
    Vector2 mouse = Raylib.GetMousePosition();

    // Gestión de Clics Centralizada (Evita que el clic "atraviese" capas)
    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        if (game.CartaInspeccionada != null)
        {
            // Si hay una carta abierta, el clic izquierdo fuera del botón de poder la cierra
            Rectangle btnPoder = new Rectangle(300, 520, 200, 50);
            if (!Raylib.CheckCollisionPointRec(mouse, btnPoder)) game.CartaInspeccionada = null;
        }
        else if (game.Estado == EstadoJuego.JUGANDO)
        {
            ManejarSeleccionDeCartas(game, mouse);
        }
    }

    // --- 2. DIBUJO ---
    Raylib.BeginDrawing();
    Raylib.ClearBackground(new Color(15, 45, 20, 255));

    switch (game.Estado)
    {
        case EstadoJuego.MENU_INICIAL:
            Raylib.DrawText("RITUAL 21", 280, 100, 50, Color.Gold);
            if (GuiHelper.DibujarBoton(300, 250, 200, 60, "JUGAR", Color.Maroon))
            {
                game.InicializarMazo();
                game.Estado = EstadoJuego.REGISTRO_CANTIDAD;
                game.InputTexto = "";
            }
            if (GuiHelper.DibujarBoton(300, 330, 200, 60, "SALIR", Color.DarkGray)) Environment.Exit(0);
            break;

        case EstadoJuego.REGISTRO_CANTIDAD:
            GuiHelper.DibujarCajaTexto(300, 250, 200, game.InputTexto, "¿CUÁNTOS JUGADORES? (2-5)");
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && game.InputTexto.Length > 0)
            {
                if (int.TryParse(game.InputTexto, out game.TotalJugadores) && game.TotalJugadores >= 2)
                {
                    game.Estado = EstadoJuego.REGISTRO_NOMBRES;
                    game.InputTexto = "";
                }
            }
            if (GuiHelper.DibujarBoton(20, 530, 100, 40, "ATRÁS", Color.DarkBlue)) game.Estado = EstadoJuego.MENU_INICIAL;
            break;

        case EstadoJuego.REGISTRO_NOMBRES:
            GuiHelper.DibujarCajaTexto(250, 250, 300, game.InputTexto, $"NOMBRE DEL INVOCADOR {game.Participantes.Count + 1}:");
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && game.InputTexto.Length > 0)
            {
                game.Participantes.Add(new Jugador(game.InputTexto));
                game.InputTexto = "";
                if (game.Participantes.Count == game.TotalJugadores) game.ComenzarDuelo();
            }
            if (GuiHelper.DibujarBoton(20, 530, 100, 40, "ATRÁS", Color.DarkBlue)) { game.Participantes.Clear(); game.Estado = EstadoJuego.REGISTRO_CANTIDAD; }
            break;

        case EstadoJuego.ESPERA_TURNO:
            Raylib.DrawRectangle(0, 0, 800, 600, Color.Black);
            Raylib.DrawText($"TURNO DE: {game.Participantes[game.IndiceActual].Nombre.ToUpper()}", 220, 250, 30, Color.Gold);
            Raylib.DrawText("Haz clic para continuar tu ritual", 260, 320, 15, Color.Gray);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) game.Estado = EstadoJuego.JUGANDO;
            break;

        case EstadoJuego.JUGANDO:
            DibujarEscenaJuego(game);
            // La inspección se dibuja al final para que esté sobre los botones
            if (game.CartaInspeccionada != null) DibujarInspeccion(game);
            break;

        case EstadoJuego.FINAL:
            DibujarPantallaFinal(game);
            break;
    }

    Raylib.EndDrawing();
}

VisualResourceManager.DescargarRecursos();
Raylib.CloseWindow();

// --- FUNCIONES DE SOPORTE ---

void ManejarSeleccionDeCartas(GameController g, Vector2 mouse)
{
    var actual = g.Participantes[g.IndiceActual];
    for (int i = 0; i < actual.Mano.Count; i++)
    {
        Vector2 pos = new Vector2(50 + (i * 110), 140);
        if (Raylib.CheckCollisionPointRec(mouse, new Rectangle(pos.X, pos.Y, 100, 150)))
        {
            g.CartaInspeccionada = actual.Mano[i];
            break;
        }
    }
}

void DibujarInspeccion(GameController g)
{
    if (g.CartaInspeccionada == null) return;
    
    Raylib.DrawRectangle(0, 0, 800, 600, new Color(0, 0, 0, 245)); // Fondo más oscuro

    Texture2D tex = GuiHelper.ObtenerTextura(g.CartaInspeccionada!);
    Vector2 posCentro = new Vector2(400 - (tex.Width * 0.75f) / 2, 300 - (tex.Height * 0.75f) / 2);
    Raylib.DrawTextureEx(tex, posCentro, 0, 0.75f, Color.White);

    if (g.CartaInspeccionada.TieneHabilidad && !g.CartaInspeccionada.HabilidadUsada)
    {
        if (GuiHelper.DibujarBoton(300, 520, 200, 50, "ACTIVAR PODER", Color.Gold))
        {
            g.ActivarHabilidadInspeccionada();
        }
    }

    Raylib.DrawText("CLICK DERECHO O CLICK FUERA PARA CERRAR", 240, 40, 15, Color.Gray);
    if (Raylib.IsMouseButtonPressed(MouseButton.Right)) g.CartaInspeccionada = null;
}

void DibujarEscenaJuego(GameController g)
{
    for (int i = 0; i < g.Participantes.Count; i++)
    {
        Color c = (i == g.IndiceActual) ? Color.Gold : Color.White;
        Raylib.DrawText($"{g.Participantes[i].Nombre}: {g.Participantes[i].Puntos}", 20 + (i * 150), 20, 18, c);
    }

    var actual = g.Participantes[g.IndiceActual];
    Raylib.DrawText($"INVOCADOR ACTUAL: {actual.Nombre.ToUpper()}", 20, 80, 22, Color.Gold);

    // Dibujar Mano
    for (int i = 0; i < actual.Mano.Count; i++)
    {
        Vector2 pos = new Vector2(50 + (i * 110), 140);
        GuiHelper.DibujarCarta(actual.Mano[i], pos, 0.25f);
        Raylib.DrawText($"{actual.Mano[i].Valor}", (int)pos.X + 40, (int)pos.Y + 160, 20, Color.SkyBlue);
    }

    // INTERFAZ DE BOTONES DINÁMICA
    if (g.CartaInspeccionada == null)
    {
        if (!g.RealizoAccionEsteTurno)
        {
            if (!g.BloqueadoPorAnimacion && GuiHelper.DibujarBoton(200, 480, 180, 60, "PEDIR", Color.Maroon))
            {
                g.RobarPara(actual);
                g.BloqueadoPorAnimacion = true;
                g.TiempoEspera = Raylib.GetTime() + 0.8;
            }

            if (GuiHelper.DibujarBoton(420, 480, 180, 60, "QUEDARSE", Color.DarkBlue))
            {
                actual.SePlanto = true;
                g.PasarSiguiente();
            }
        }
        else
        {
            // Solo sale este botón si ya pidió
            if (GuiHelper.DibujarBoton(310, 480, 200, 60, "TERMINAR TURNO", Color.DarkGreen))
            {
                g.PasarSiguiente();
            }
        }
    }

    foreach (var anim in g.AnimacionesActivas)
    {
        GuiHelper.DibujarCarta(anim.Info, anim.PosicionActual, anim.EscalaActual);
    }
}

void DibujarPantallaFinal(GameController g)
{
    var ganador = g.Participantes.Where(j => j.Puntos <= 21).OrderByDescending(j => j.Puntos).FirstOrDefault();
    Raylib.DrawText("RITUAL COMPLETADO", 220, 150, 35, Color.Gold);
    Raylib.DrawText(ganador != null ? $"¡{ganador.Nombre.ToUpper()} GANA!" : "NADIE SOBREVIVIÓ", 210, 230, 25, Color.White);
    if (GuiHelper.DibujarBoton(300, 320, 200, 50, "REVANCHA", Color.Maroon)) g.ComenzarDuelo();
    if (GuiHelper.DibujarBoton(300, 390, 200, 50, "MENÚ", Color.DarkBlue)) g.Estado = EstadoJuego.MENU_INICIAL;
}