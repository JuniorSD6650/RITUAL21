using Raylib_cs;

namespace Ritual21.Core;

public static class VisualResourceManager
{
    // Diccionario para guardar las texturas y no cargarlas mil veces (eficiencia)
    private static Dictionary<string, Texture2D> _texturas = new();

    public static void CargarRecursos(string paquete)
    {
        string rutaBase = $"Assets/{paquete}/";
        
        // Cargamos cada uno de tus 7 diseños
        _texturas["sbirro"] = Raylib.LoadTexture(rutaBase + "sbirro.png");
        _texturas["guardian"] = Raylib.LoadTexture(rutaBase + "guardian.png");
        _texturas["espectro"] = Raylib.LoadTexture(rutaBase + "espectro.png");
        _texturas["chaman"] = Raylib.LoadTexture(rutaBase + "chaman.png");
        _texturas["quimera"] = Raylib.LoadTexture(rutaBase + "quimera.png");
        _texturas["beholder"] = Raylib.LoadTexture(rutaBase + "beholder.png");
        _texturas["dorso"] = Raylib.LoadTexture(rutaBase + "dorso.png");
    }

    public static Texture2D ObtenerTextura(string nombre) => _texturas[nombre];

    public static void DescargarRecursos()
    {
        foreach (var tex in _texturas.Values) Raylib.UnloadTexture(tex);
    }
}