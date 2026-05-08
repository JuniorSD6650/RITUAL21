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
    game.Actualizar();
    Vector2 mouse = Raylib.GetMousePosition();

    // Gestión táctica de clics (Optimizado para APK)
    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        if (game.CartaInspeccionada != null)
        {
            Rectangle btnPoder = new Rectangle(300, 520, 200, 50);
            if (!Raylib.CheckCollisionPointRec(mouse, btnPoder)) game.CartaInspeccionada = null;
        }
        else if (game.Estado == EstadoJuego.JUGANDO && !game.BloqueadoPorAnimacion)
        {
            ManejarSeleccionDeCartas(game, mouse);
        }
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(new Color(15, 35, 20, 255));

    switch (game.Estado)
    {
        case EstadoJuego.MENU_INICIAL:
            Raylib.DrawText("RITUAL 21", 280, 150, 50, Color.Gold);
            if (GuiHelper.DibujarBoton(300, 300, 200, 60, "JUGAR", Color.Maroon)) {
                game.InicializarMazo();
                game.Estado = EstadoJuego.REGISTRO_CANTIDAD;
            }
            if (GuiHelper.DibujarBoton(300, 380, 200, 60, "SALIR", Color.DarkGray)) Environment.Exit(0);
            break;

        case EstadoJuego.REGISTRO_CANTIDAD:
            GuiHelper.DibujarCajaTexto(300, 250, 200, game.InputTexto, "¿CUÁNTOS JUGADORES? (2-5)");
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && game.InputTexto.Length > 0) {
                if (int.TryParse(game.InputTexto, out game.TotalJugadores) && game.TotalJugadores >= 2) {
                    game.Estado = EstadoJuego.REGISTRO_NOMBRES;
                    game.InputTexto = "";
                }
            }
            break;

        case EstadoJuego.REGISTRO_NOMBRES:
            GuiHelper.DibujarCajaTexto(250, 250, 300, game.InputTexto, $"NOMBRE JUGADOR {game.Participantes.Count + 1}:");
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && game.InputTexto.Length > 0) {
                game.Participantes.Add(new Jugador(game.InputTexto));
                game.InputTexto = "";
                if (game.Participantes.Count == game.TotalJugadores) game.ComenzarDuelo();
            }
            break;

        case EstadoJuego.ESPERA_TURNO:
            Raylib.DrawRectangle(0, 0, 800, 600, Color.Black);
            Raylib.DrawText($"TURNO DE: {game.Participantes[game.IndiceActual].Nombre.ToUpper()}", 220, 250, 30, Color.Gold);
            Raylib.DrawText("Toca para iniciar el ritual", 285, 320, 15, Color.Gray);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) game.Estado = EstadoJuego.JUGANDO;
            break;

        case EstadoJuego.JUGANDO:
            DibujarEscenaJuego(game);
            if (game.CartaInspeccionada != null) DibujarInspeccion(game);
            break;

        case EstadoJuego.FINAL:
            DibujarPantallaFinal(game);
            break;
    }
    Raylib.EndDrawing();
}

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
    Raylib.DrawRectangle(0, 0, 800, 600, new Color(0, 0, 0, 235));
    Texture2D tex = GuiHelper.ObtenerTextura(g.CartaInspeccionada!);
    Vector2 pos = new Vector2(400 - (tex.Width * 0.75f) / 2, 300 - (tex.Height * 0.75f) / 2);
    Raylib.DrawTextureEx(tex, pos, 0, 0.75f, Color.White);

    if (g.CartaInspeccionada!.TieneHabilidad && !g.CartaInspeccionada.HabilidadUsada)
    {
        if (GuiHelper.DibujarBoton(300, 520, 200, 50, "USAR PODER", Color.Gold)) g.ActivarHabilidadInspeccionada();
    }
}

void DibujarEscenaJuego(GameController g)
{
    var actual = g.Participantes[g.IndiceActual];
    for (int i = 0; i < g.Participantes.Count; i++)
        Raylib.DrawText($"{g.Participantes[i].Nombre[..1]}: {g.Participantes[i].Puntos}", 20 + (i * 90), 20, 20, (i == g.IndiceActual ? Color.Gold : Color.White));

    for (int i = 0; i < actual.Mano.Count; i++) GuiHelper.DibujarCarta(actual.Mano[i], new Vector2(50 + (i * 110), 140), 0.25f);
    foreach (var anim in g.AnimacionesActivas) GuiHelper.DibujarCarta(anim.Info, anim.PosicionActual, anim.EscalaActual);

    if (g.CartaInspeccionada == null && !g.BloqueadoPorAnimacion) {
        if (!g.RealizoAccionEsteTurno) {
            if (GuiHelper.DibujarBoton(150, 480, 240, 70, "PEDIR", Color.Maroon)) g.RobarConAnimacion(actual);
            if (GuiHelper.DibujarBoton(410, 480, 240, 70, "QUEDARSE", Color.DarkBlue)) { actual.SePlanto = true; g.PasarSiguiente(); }
        } else if (GuiHelper.DibujarBoton(280, 480, 240, 70, "TERMINAR", Color.DarkGreen)) g.PasarSiguiente();
    }
}

void DibujarPantallaFinal(GameController g)
{
    // BUSCAR GANADOR: El que tenga más puntos sin pasarse de 21
    var ganador = g.Participantes
        .Where(j => j.Puntos <= 21)
        .OrderByDescending(j => j.Puntos)
        .FirstOrDefault();

    Raylib.DrawText("RITUAL FINALIZADO", 225, 150, 35, Color.Gold);

    string mensaje = (ganador != null) 
        ? $"¡EL GANADOR ES {ganador.Nombre.ToUpper()}!" 
        : "EL VACÍO RECLAMÓ A TODOS";
    
    // Centramos el texto dinámicamente
    int textoAncho = Raylib.MeasureText(mensaje, 25);
    Raylib.DrawText(mensaje, 400 - textoAncho / 2, 240, 25, Color.White);

    if (GuiHelper.DibujarBoton(300, 340, 200, 55, "REVANCHA", Color.Maroon)) g.ComenzarDuelo();
    if (GuiHelper.DibujarBoton(300, 410, 200, 55, "MENÚ", Color.DarkBlue)) g.Estado = EstadoJuego.MENU_INICIAL;
}