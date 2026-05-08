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

        // Efecto visual: si se presiona (toque), se oscurece
        Color colorFinal = hover ? Color.Red : col;
        if (hover && Raylib.IsMouseButtonDown(MouseButton.Left)) colorFinal = Color.Maroon;

        Raylib.DrawRectangleRec(rect, colorFinal);
        Raylib.DrawRectangleLinesEx(rect, 2, Color.Gold); // Borde dorado profesional

        int fontSize = 20;
        int textX = x + (w / 2 - Raylib.MeasureText(texto, fontSize) / 2);
        int textY = y + h / 2 - 10;
        Raylib.DrawText(texto, textX, textY, fontSize, Color.White);

        return hover && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }

    public static void DibujarCajaTexto(int x, int y, int w, string texto, string titulo)
    {
        Raylib.DrawText(titulo, x, y - 35, 20, Color.Gold);
        Raylib.DrawRectangle(x, y, w, 50, Color.Black);
        Raylib.DrawRectangleLinesEx(new Rectangle(x, y, w, 50), 2, Color.Gold);
        Raylib.DrawText(texto + "|", x + 15, y + 10, 30, Color.RayWhite);
    }

    public static void DibujarCarta(Carta carta, Vector2 posicion, float escala)
    {
        Texture2D tex = ObtenerTextura(carta);

        Raylib.DrawTextureEx(tex, posicion + new Vector2(4, 4), 0, escala, new Color(0, 0, 0, 100));

        Raylib.DrawTextureEx(tex, posicion, 0, escala, Color.White);

        string textoValor = carta.Valor.ToString();

        int fontSize = (int)(75 * escala);
        if (fontSize < 18) fontSize = 18; 

        int anchoCartaEscalada = (int)(tex.Width * escala);
        int textoAncho = Raylib.MeasureText(textoValor, fontSize);

        int posXTexto = (int)posicion.X + (anchoCartaEscalada / 2 - textoAncho / 2);
        int posYTexto = (int)posicion.Y + (int)(tex.Height * escala) + 5;

        Raylib.DrawText(textoValor, posXTexto, posYTexto, fontSize, Color.SkyBlue);
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