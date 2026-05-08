using Raylib_cs;
using System.Numerics;
using Ritual21.Models;
using Ritual21.Core;
using Ritual21.SpecialCards;

namespace Ritual21.UI;

public static class GuiHelper
{
    public static bool DibujarBoton(int x, int y, int w, int h, string texto, Color col)
    {
        Rectangle rect = new Rectangle(x, y, w, h);
        bool hover = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect);
        Raylib.DrawRectangleRec(rect, hover ? Color.Red : col);
        Raylib.DrawText(texto, x + (w / 2 - Raylib.MeasureText(texto, 20) / 2), y + h / 2 - 10, 20, Color.White);
        return hover && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }

    public static void DibujarCajaTexto(int x, int y, int w, string texto, string titulo)
    {
        Raylib.DrawText(titulo, x, y - 35, 20, Color.Gold);
        Raylib.DrawRectangle(x, y, w, 50, Color.Black);
        Raylib.DrawRectangleLines(x, y, w, 50, Color.Gold);
        Raylib.DrawText(texto + "_", x + 15, y + 10, 30, Color.RayWhite);
    }

    public static void DibujarCarta(Carta carta, Vector2 posicion, float escala)
    {
        Texture2D tex = ObtenerTextura(carta);
        Raylib.DrawTextureEx(tex, posicion, 0, escala, Color.White);
    }

    public static Texture2D ObtenerTextura(Carta c)
    {
        if (c is Beholder) return VisualResourceManager.ObtenerTextura("beholder");
        if (c is Espectro) return VisualResourceManager.ObtenerTextura("espectro");
        if (c is Chaman) return VisualResourceManager.ObtenerTextura("chaman");
        if (c.Valor == 10 && c.Nombre.Contains("Guardian")) return VisualResourceManager.ObtenerTextura("guardian");
        return VisualResourceManager.ObtenerTextura("sbirro");
    }
}